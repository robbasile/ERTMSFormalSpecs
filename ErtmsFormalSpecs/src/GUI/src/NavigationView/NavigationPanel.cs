using System.Collections.Generic;
using System.Drawing;
using DataDictionary;
using GUI.BoxArrowDiagram;
using Utils;

namespace GUI.NavigationView
{
    public class NavigationPanel : BoxArrowPanel<INamable, NamableWrapper, IGraphicalArrow<NamableWrapper>>
    {
        /// <summary>
        /// Creates a box for this display
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<INamable, NamableWrapper, IGraphicalArrow<NamableWrapper>> CreateBox(NamableWrapper model)
        {
            return new NavigationControl(this, model);
        }

        /// <summary>
        /// No arrows in this view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<INamable, NamableWrapper, IGraphicalArrow<NamableWrapper>> CreateArrow(IGraphicalArrow<NamableWrapper> model)
        {
            return null;
        }

        /// <summary>
        /// Provides the boxes for this display
        /// </summary>
        /// <returns></returns>
        public override List<NamableWrapper> GetBoxes()
        {
            List<NamableWrapper> retVal = new List<NamableWrapper>();

            INamable current = Model;
            while (current != null && !(current is EfsSystem))
            {
                retVal.Insert(0, new NamableWrapper(current));

                IEnclosed enclosed = current as IEnclosed;
                if (enclosed != null)
                {
                    current = enclosed.Enclosing as INamable;
                }
                else
                {
                    current = null;
                }
            }

            return retVal;
        }

        /// <summary>
        /// No arrows in this view
        /// </summary>
        /// <returns></returns>
        public override List<IGraphicalArrow<NamableWrapper>> GetArrows()
        {
            return new List<IGraphicalArrow<NamableWrapper>>();
        }

        /// <summary>
        /// Sets the box positions in the view
        /// </summary>
        protected override void UpdateBoxPosition()
        {
            int x = 3;
            int y = 3;

            int width = 0;
            int height = 0;

            foreach (var box in _boxes.Values)
            {
                box.Location = new Point(x, y);
                x = x + box.Width + 2;

                width = x;
                height = box.Height + 2 * y;
            }

            pictureBox.Size = new Size(width, height);
        }
    }
}
