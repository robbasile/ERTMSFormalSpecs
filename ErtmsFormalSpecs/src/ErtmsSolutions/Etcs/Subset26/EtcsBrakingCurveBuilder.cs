// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Reflection;
using ErtmsSolutions.SiUnits;
using log4net;

namespace ErtmsSolutions.Etcs.Subset26.BrakingCurves
{
    public static class EtcsBrakingCurveBuilder
    {
        /************************************************************/
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static bool debug = false;
        private static int debugging_counter = 0;
        private static SiSpeed minimal_speed_threshold = new SiSpeed(0.01);
        private static SiDistance minimal_distance_threshold = new SiDistance(0.1);

        /// <summary>
        ///     Method used to get the full EBD and SBD curves from the MRSP
        /// </summary>
        /// <param name="A_V_D"></param>
        /// <param name="MRSP"></param>
        /// <returns></returns>
        public static QuadraticSpeedDistanceCurve Build_A_Safe_Backward(AccelerationSpeedDistanceSurface A_V_D,
            FlatSpeedDistanceCurve MRSP)
        {
            debugging_counter = 0;

            if (debug)
            {
                Log.InfoFormat("#######################################################");
                Log.InfoFormat("## Build_A_Safe_Backward_Surface#######################");
                Log.InfoFormat("#######################################################");
            }

            QuadraticSpeedDistanceCurve result = new QuadraticSpeedDistanceCurve();

            /*********************************************************** 
              The ending point is the first point in the MRSP
             ***********************************************************/
            SiDistance end_position = MRSP[0].X.X0;


            /*********************************************************** 
              Go forward in the MRSP until we find the point with
              minimal speed. This shall be our starting point
              **********************************************************/
            SiDistance current_position = SiDistance.Zero;
            SiSpeed current_speed = SiSpeed.MaxValue;

            if (debug)
                Log.DebugFormat("  Search the position of the minimal speed in the MRSP");
            for (int i = 0; i < MRSP.SegmentCount; i++)
            {
                ConstantCurveSegment<SiDistance, SiSpeed> segment = MRSP[i];
                if (segment.Y < current_speed)
                {
                    current_speed = segment.Y;
                    current_position = segment.X.X1;

                    if (debug)
                        Log.DebugFormat("    new start position V={0,7:F2} at={1,7:F2} ", current_speed.ToUnits(),
                            current_position.ToUnits());
                }
            }

            if (debug)
                Log.DebugFormat("    end position is at={0,7:F2} ", end_position.ToUnits());

            SiDistance next_position = SiDistance.Zero;

            /*************************************************************************/
            /* Starting from the right side of curves, go back to the left side.     */
            /* Build small curves arcs where the acceleration is constant on each one*/
            /*************************************************************************/
            while (current_position > end_position)
            {
                Compute_Curve(A_V_D, result, MRSP, ref current_position, ref next_position, ref current_speed,
                    BrakingCurveDirectionEnum.Backwards);

                /* Next loop starts from our new position.
                   We do not need to update current_acceleration because
                   it is done at the beginning of the loop*/
                current_position = next_position;
                current_speed = result.GetValueAt(current_position, BrakingCurveDirectionEnum.Backwards);


                /*************************************************************/
                /* If this exception is thrown, you'd better call Juan       */
                /*************************************************************/
                if (debugging_counter++ > 200)
                {
                    throw new Exception("Algorithm is broken");
                }
            }

            return result;
        }


        /// <summary>
        ///     Builds a full deceleration curve corresponding to a given target(location, speed)
        /// </summary>
        /// <param name="A_V_D"></param>
        /// <param name="TargetSpeed"></param>
        /// <param name="TargetDistance"></param>
        /// <returns></returns>
        public static QuadraticSpeedDistanceCurve Build_Deceleration_Curve(AccelerationSpeedDistanceSurface A_V_D,
            SiSpeed TargetSpeed, SiDistance TargetDistance)
        {
            debugging_counter = 0;

            QuadraticSpeedDistanceCurve result = new QuadraticSpeedDistanceCurve();

            // Build a MRSP for this target
            FlatSpeedDistanceCurve mrsp = new FlatSpeedDistanceCurve();
            mrsp.Add(SiDistance.Zero, TargetDistance, new SiSpeed(400, SiSpeed_SubUnits.KiloMeter_per_Hour));
            mrsp.Add(TargetDistance, new SiDistance(20000), TargetSpeed);

            // Add to result by calculating backwards then forwards

            SiDistance current_position = TargetDistance;
            SiSpeed current_speed = TargetSpeed;
            BrakingCurveDirectionEnum dir = BrakingCurveDirectionEnum.Backwards;
            SiDistance next_position = SiDistance.Zero;

            SiDistance end_position = mrsp[0].X.X0;

            while (current_position > end_position)
            {
                Compute_Curve(A_V_D, result, mrsp, ref current_position, ref next_position, ref current_speed, dir);

                /* Next loop starts from our new position.
                   We do not need to update current_acceleration because
                   it is done at the beginning of the loop*/
                current_position = next_position;
                current_speed = result.GetValueAt(current_position, BrakingCurveDirectionEnum.Backwards);
            }


            current_position = TargetDistance;
            current_speed = TargetSpeed;
            dir = BrakingCurveDirectionEnum.Forwards;

            while (current_speed > minimal_speed_threshold)
            {
                Compute_Curve(A_V_D, result, mrsp, ref current_position, ref next_position, ref current_speed, dir);

                /* Next loop starts from our new position.
                   We do not need to update current_acceleration because
                   it is done at the beginning of the loop*/
                current_position = next_position;
                current_speed = result.GetValueAt(current_position, dir);
            }

            result.Add(current_position, mrsp[mrsp.SegmentCount - 1].X.X1, SiAcceleration.Zero, SiSpeed.Zero,
                current_position);

            return result;
        }


        /// <summary>
        ///     Computes the curve from a point
        /// </summary>
        /// <param name="A_V_D"></param>
        /// <param name="result"></param>
        /// <param name="mrsp"></param>
        /// <param name="current_position"></param>
        /// <param name="next_position"></param>
        /// <param name="current_speed"></param>
        /// <param name="dir"></param>
        private static void Compute_Curve(AccelerationSpeedDistanceSurface A_V_D,
            QuadraticSpeedDistanceCurve result,
            FlatSpeedDistanceCurve mrsp,
            ref SiDistance current_position,
            ref SiDistance next_position,
            ref SiSpeed current_speed,
            BrakingCurveDirectionEnum dir)
        {
            int Direction = Get_Direction(dir);

            SiSpeed speed_step = (-1)*Direction*minimal_speed_threshold;
            SiDistance distance_step = Direction*minimal_distance_threshold;


            if (debug)
            {
                Log.DebugFormat("#######################################################");
                Log.DebugFormat("### Loop {0}  #########################################", debugging_counter);
                Log.DebugFormat("#######################################################");
            }

            /************************************************************ 
              Based on current speed and position, search on wich tile
              of A_V_D tile we are
              ***********************************************************/
            SurfaceTile current_tile = A_V_D.GetTileAt(current_speed + speed_step, current_position + distance_step);

            /***************************************************************************/
            /* If at previous loop wi did 'hit' the vertical part of the MRSP,
               we might have a speed above the current MRSP segment.*/
            /***************************************************************************/
            if (current_speed >
                mrsp.GetValueAt(current_position - minimal_distance_threshold, BrakingCurveDirectionEnum.Backwards))
            {
                current_speed = mrsp.GetValueAt(current_position - minimal_distance_threshold,
                    BrakingCurveDirectionEnum.Backwards);
            }

            /******************************************************************* 
              We build a quadratic arc with current train position, speed
              and acceleration. The arc domain [0..current_position] is not valid yet.
              We must find out the domain left limit.
             *****************************************************************/
            QuadraticCurveSegment current_curve = Build_One_Curve_Segment(current_tile, current_position, current_speed,
                mrsp, dir);

            next_position = Distance_Edge(current_curve, dir);
            SiAcceleration current_acceleration = current_curve.A;

            /* Finally we can add the segment because next_position has been computed. */
            SiDistance refLocation = current_curve.X.X0;
            SiSpeed refSpeed = current_curve.Get(refLocation);
            result.Add(current_curve.X.X0, current_curve.X.X1, current_acceleration, refSpeed, refLocation);

            result.Dump("result so far ");
        }

        /// <summary>
        ///     Converts the direction to a positive or negative integer, for use in the speed and distance steps
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static int Get_Direction(BrakingCurveDirectionEnum dir)
        {
            int result = 0;
            switch (dir)
            {
                case BrakingCurveDirectionEnum.Forwards:
                    result = 1;
                    break;
                case BrakingCurveDirectionEnum.Backwards:
                    result = -1;
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Method to return the curve segment at the current location and speed, going in the specified direction.
        /// </summary>
        /// <param name="current_tile"></param>
        /// <param name="current_position"></param>
        /// <param name="current_speed"></param>
        /// <param name="MRSP"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static QuadraticCurveSegment Build_One_Curve_Segment(SurfaceTile current_tile,
            SiDistance current_position,
            SiSpeed current_speed,
            FlatSpeedDistanceCurve MRSP,
            BrakingCurveDirectionEnum dir)
        {
            SiAcceleration current_acceleration = current_tile.V.Y;
            SiDistance MRSP_end = MRSP[MRSP.SegmentCount - 1].X.X1;

            SiDistance curve_start = new SiDistance();
            SiDistance curve_end = new SiDistance();
            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    curve_start = SiDistance.Zero;
                    curve_end = current_position;
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    curve_start = current_position;
                    curve_end = MRSP_end;
                    break;
            }


            QuadraticCurveSegment current_curve = new QuadraticCurveSegment(curve_start,
                curve_end,
                current_acceleration,
                current_speed,
                current_position);


            if (debug)
            {
                Log.DebugFormat("  current_acceleration = {0,7:F2} from a_tile {1}", current_acceleration.ToUnits(),
                    current_tile.ToString());
                Log.DebugFormat("  current_speed        = {0,7:F2} ", current_speed.ToUnits());
                Log.DebugFormat("  current_position     = {0,7:F2} ", current_position.ToUnits());

                Log.DebugFormat("  --> current_curve    = {0} ", current_curve.ToString());
            }


            /********************************************************************/
            /* The current_curve may 'hit' one of these 4 items:
                    1) The upper border of the tile (because of a new acceleration) 
                    2) The left border of the tile (because of a gradient?)
                    3) A vertical segment of the MRSP                                                           
                    4) An horizontal segment of the MRSP
                Text all of them and update the next_position accordingly.
            *************************************************************************/
            SiDistance next_position = SiDistance.Zero;

            /* The distance at which our temporary arc intersects a segment of the AVD tile */
            {
                next_position = Tile_Intersect(current_position, current_tile, current_curve, dir);
            }


            /* The MRSP checks only need to be performed if the curve is being computed backwards */
            if (dir == BrakingCurveDirectionEnum.Backwards)
            {
                /*Since the MRSP is continous, the following cannot fail. */
                ConstantCurveSegment<SiDistance, SiSpeed> speed_limit_here =
                    MRSP.Intersect(current_position - new SiDistance(0.1), current_curve);
                if (debug)
                    Log.DebugFormat("  MRSP segment          {0} ", speed_limit_here.ToString());

                /* 3) Do we hit the vertical segment of the MRSP ? */
                {
                    next_position = IntersectMRSPSpeed(next_position, speed_limit_here);
                }

                /* 4) Do we hit the horizontal segment of the MRSP */
                {
                    if (current_speed + new SiSpeed(0.01) < speed_limit_here.Y)
                    {
                        SiDistance d = current_curve.IntersectAt(speed_limit_here.Y);
                        if (d >= next_position)
                        {
                            if (debug)
                                Log.DebugFormat("  --> case_4a next_d        {0,7:F2} -> {1,7:F2}",
                                    next_position.ToUnits(), d.ToUnits());
                            next_position = d;
                        }
                    }
                    else
                    {
                        if (debug)
                            Log.DebugFormat("  --> case_4b next_acc_0    {0,7:F2} -> {1,7:F2}", next_position.ToUnits(),
                                speed_limit_here.X.X0.ToUnits());
                        current_acceleration = SiAcceleration.Zero;
                        next_position = speed_limit_here.X.X0;
                    }
                }
            }

            SiDistance result_start = new SiDistance();
            SiDistance result_end = new SiDistance();

            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    result_start = next_position;
                    result_end = current_position;
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    result_start = current_position;
                    result_end = next_position;
                    break;
            }

            QuadraticCurveSegment result = new QuadraticCurveSegment(result_start,
                result_end,
                current_acceleration,
                current_speed,
                current_position);

            return result;
        }

        /// <summary>
        ///     Finds the tile edge that is intersected in the chosen direction
        /// </summary>
        /// <param name="current_tile"></param>
        /// <param name="current_curve"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SiDistance Tile_Intersect(SiDistance current_position, SurfaceTile current_tile,
            QuadraticCurveSegment current_curve, BrakingCurveDirectionEnum dir)
        {
            SiDistance result = SiDistance.Zero;

            SiDistance SpdEdge = Speed_Edge_Location(current_tile, current_curve, dir);
            SiDistance DistEdge = Distance_Edge(current_tile, dir);

            result = Closest_To(current_position, DistEdge, SpdEdge);

            return result;
        }

        /// <summary>
        ///     Takes three SiDistances and determines which of the second two is closest to the first one
        /// </summary>
        /// <param name="reference">The point the others are compared to.</param>
        /// <param name="first">The first compared distance.</param>
        /// <param name="second">The second compared distance.</param>
        /// <returns></returns>
        private static SiDistance Closest_To(SiDistance reference, SiDistance first, SiDistance second)
        {
            SiDistance result = first;

            if (Math.Abs(reference.Value - first.Value) > Math.Abs(reference.Value - second.Value))
            {
                result = second;
            }

            return result;
        }

        /// <summary>
        ///     Provides the distance at which the curve intersects the horizontal edge of the tile in the given direction
        /// </summary>
        /// <param name="current_tile"></param>
        /// <param name="current_curve"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SiDistance Speed_Edge_Location(SurfaceTile current_tile, QuadraticCurveSegment current_curve,
            BrakingCurveDirectionEnum dir)
        {
            SiDistance result = SiDistance.Zero;

            SiSpeed limit = SiSpeed.Zero;

            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    limit = current_tile.V.X.X1;
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    limit = current_tile.V.X.X0;
                    break;
            }
            result = current_curve.IntersectAt(limit);

            return result;
        }

        /// <summary>
        ///     Provides the distance limit of a tile in the requested direction
        /// </summary>
        /// <param name="current_tile"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SiDistance Distance_Edge(SurfaceTile current_tile, BrakingCurveDirectionEnum dir)
        {
            SiDistance result = SiDistance.Zero;

            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    result = current_tile.D.X.X0;
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    result = current_tile.D.X.X1;
                    break;
            }

            return result;
        }


        /// <summary>
        ///     Provides the distance limit of a tile in the requested direction
        /// </summary>
        /// <param name="current_tile"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SiDistance Distance_Edge(QuadraticCurveSegment current_curve, BrakingCurveDirectionEnum dir)
        {
            SiDistance result = SiDistance.Zero;

            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    result = current_curve.X.X0;
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    result = current_curve.X.X1;
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Finds the distance at which the curve intersects the MRSP speed
        /// </summary>
        /// <param name="next_position"></param>
        /// <param name="speed_limit_here"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SiDistance IntersectMRSPSpeed(SiDistance next_position,
            ConstantCurveSegment<SiDistance, SiSpeed> speed_limit_here)
        {
            if (speed_limit_here.X.X0 >= next_position)
            {
                if (debug)
                    Log.DebugFormat("  --> case_3  next_position {0,7:F2} -> {1,7:F2}", next_position.ToUnits(),
                        speed_limit_here.X.X0.ToUnits());
                next_position = speed_limit_here.X.X0;
            }
            return next_position;
        }


        /// <summary>
        ///     Provides the current tile based on the direction we are looking in
        /// </summary>
        /// <param name="A_V_D"></param>
        /// <param name="location"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static SurfaceTile Get_Current_Tile(AccelerationSpeedDistanceSurface A_V_D, SiDistance current_location,
            SiSpeed current_speed, BrakingCurveDirectionEnum dir)
        {
            foreach (SurfaceTile Tile in A_V_D.Tiles)
            {
                if (TileContains(Tile, current_location, current_speed, dir))
                    return Tile;
            }

            return null;
        }

        /// <summary>
        ///     Determines whether a tile conatins the provided speed and distance, based on the direction in which we are looking
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="current_location"></param>
        /// <param name="current_speed"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static bool TileContains(SurfaceTile tile, SiDistance current_location, SiSpeed current_speed,
            BrakingCurveDirectionEnum dir)
        {
            bool result = false;

            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    result = (tile.D.X.X0.ToUnits() < current_location.ToUnits() &&
                              current_location.ToUnits() <= tile.D.X.X1.ToUnits() &&
                              tile.V.X.Contains(current_speed));
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    result = (tile.D.X.Contains(current_location) &&
                              tile.V.X.X0.ToUnits() < current_speed.ToUnits() &&
                              current_speed.ToUnits() <= tile.V.X.X0.ToUnits());
                    break;
            }

            return result;
        }
    }
}