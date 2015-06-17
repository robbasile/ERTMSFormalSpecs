using System;
using System.IO;
using System.Text;
using ErtmsSolutions.SiUnits;
using NUnit.Framework;

namespace ErtmsSolutions.Etcs.Subset26.BrakingCurves
{
    [TestFixture]
    public class EtcsBrakingCurveBuilder_UnitTest
    {
        private StringBuilder TestOutput;

        [Test]
        public void Build_A_Safe_BackwardTestMethod()
        {
            // TODO : Fill the acceleration surface
            AccelerationSpeedDistanceSurface acceleration = new AccelerationSpeedDistanceSurface();

            for (int i = 0; i < 5; i++)
            {
                double StartDistance = i*1000;
                double EndDistance = StartDistance + 1000;

                for (int j = 0; j < 4; j++)
                {
                    double StartSpeed = j*60;
                    double EndSpeed = StartSpeed + 60;

                    acceleration.Tiles.Add(new SurfaceTile(
                        new SiDistance(StartDistance),
                        new SiDistance(EndDistance),
                        new SiSpeed(StartSpeed, SiSpeed_SubUnits.KiloMeter_per_Hour),
                        new SiSpeed(EndSpeed, SiSpeed_SubUnits.KiloMeter_per_Hour),
                        new SiAcceleration(-(0.5 + (i - j)/8))
                        ));
                }
            }


            // TODO : Fill the mrsp
            FlatSpeedDistanceCurve mrsp = new FlatSpeedDistanceCurve();
            mrsp.AddSegment(new ConstantCurveSegment<SiDistance, SiSpeed>(
                new SiDistance(0),
                new SiDistance(650),
                new SiSpeed(80, SiSpeed_SubUnits.KiloMeter_per_Hour)
                ));
            mrsp.AddSegment(new ConstantCurveSegment<SiDistance, SiSpeed>(
                new SiDistance(650),
                new SiDistance(1500),
                new SiSpeed(60, SiSpeed_SubUnits.KiloMeter_per_Hour)
                ));
            mrsp.AddSegment(new ConstantCurveSegment<SiDistance, SiSpeed>(
                new SiDistance(1500),
                new SiDistance(2500),
                new SiSpeed(120, SiSpeed_SubUnits.KiloMeter_per_Hour)
                ));
            mrsp.AddSegment(new ConstantCurveSegment<SiDistance, SiSpeed>(
                new SiDistance(2500),
                new SiDistance(4000),
                new SiSpeed(55, SiSpeed_SubUnits.KiloMeter_per_Hour)
                ));
            mrsp.AddSegment(new ConstantCurveSegment<SiDistance, SiSpeed>(
                new SiDistance(4000),
                new SiDistance(5000),
                new SiSpeed(0)
                ));

            // Compute the deceleration curve using the previous algorithm
            QuadraticSpeedDistanceCurve deceleration =
                EtcsBrakingCurveBuilder_Obsolete.Build_A_Safe_Backward(acceleration, mrsp);

            // Compute the deceleration curve using the new algorithm
            // TODO : Implement the new algorithm and use it
            QuadraticSpeedDistanceCurve deceleration2 = EtcsBrakingCurveBuilder.Build_A_Safe_Backward(acceleration, mrsp);

            TestOutput = new StringBuilder();

            // Compare the deceleration curves
            for (double d = 0.0; d < 5000.0; d += 1)
            {
                Assert.AreEqual(
                    deceleration.GetValueAt(new SiDistance(0.0 + d), BrakingCurveDirectionEnum.Backwards),
                    deceleration2.GetValueAt(new SiDistance(0.0 + d), BrakingCurveDirectionEnum.Backwards),
                    "Value at " + d + " should be equal"
                    );
                TestOutput.Append(d.ToString());
                TestOutput.Append("\t");
                TestOutput.Append(mrsp.GetValueAt(new SiDistance(d), BrakingCurveDirectionEnum.Backwards).Value);
                TestOutput.Append("\t");
                TestOutput.Append(deceleration.GetValueAt(new SiDistance(d), BrakingCurveDirectionEnum.Backwards).Value);
                TestOutput.Append("\t");
                TestOutput.Append(deceleration2.GetValueAt(new SiDistance(d), BrakingCurveDirectionEnum.Backwards).Value);
                TestOutput.Append("\n");
            }
            File.WriteAllText("ResultsCompare.csv", TestOutput.ToString());
        }

        [Test]
        public void Build_Deceleration_CurveTestMethod()
        {
            // TODO : Fill the acceleration surface
            AccelerationSpeedDistanceSurface acceleration = new AccelerationSpeedDistanceSurface();

            for (int i = 0; i < 8; i++)
            {
                double StartDistance = i*625;
                double EndDistance = StartDistance + 625;

                for (int j = 0; j < 6; j++)
                {
                    double StartSpeed = j*35;
                    double EndSpeed = StartSpeed + 35;

                    acceleration.Tiles.Add(new SurfaceTile(
                        new SiDistance(StartDistance),
                        new SiDistance(EndDistance),
                        new SiSpeed(StartSpeed, SiSpeed_SubUnits.KiloMeter_per_Hour),
                        new SiSpeed(EndSpeed, SiSpeed_SubUnits.KiloMeter_per_Hour),
                        new SiAcceleration(-Math.Abs(0.5*(1.2 + Math.Pow(-1, i - j))))
                        ));
                }
            }


            // the target
            SiSpeed TargetSpeed = new SiSpeed(50, SiSpeed_SubUnits.KiloMeter_per_Hour);
            SiDistance TargetDistance = new SiDistance(2250);

            // Compute the deceleration curve using the previous algorithm
            QuadraticSpeedDistanceCurve deceleration = EtcsBrakingCurveBuilder.Build_Deceleration_Curve(acceleration,
                TargetSpeed, TargetDistance);

            TestOutput = new StringBuilder();

            // Compare the deceleration curves
            for (double d = 0.0; d < 5000.0; d += 1)
            {
                double spd = deceleration.GetValueAt(new SiDistance(d), BrakingCurveDirectionEnum.Backwards).Value;
                TestOutput.Append(d.ToString());
                TestOutput.Append("\t");
                TestOutput.Append(spd.ToString());
                TestOutput.Append("\t");
                TestOutput.Append(acceleration.GetTileAt(new SiSpeed(spd), new SiDistance(d)).V.Y.Value);
                TestOutput.Append("\n");
            }
            File.WriteAllText("Results.csv", TestOutput.ToString());
        }
    }
}