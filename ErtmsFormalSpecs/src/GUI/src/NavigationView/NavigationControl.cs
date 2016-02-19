using System;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using GUI.BoxArrowDiagram;
using Utils;
using Type = System.Type;

namespace GUI.NavigationView
{
    class NavigationControl : BoxControl<INamable, NamableWrapper, IGraphicalArrow<NamableWrapper>>
    {
        /// <summary>
        /// The bold font
        /// </summary>
        public Font Bold { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public NavigationControl(BoxArrowPanel<INamable, NamableWrapper, IGraphicalArrow<NamableWrapper>> panel, NamableWrapper model)
            : base(panel, model)
        {
            Bold = new Font(Font, FontStyle.Bold);

            SizeF typeSize = GuiUtils.Graphics.MeasureString(TypeName, Bold);
            SizeF nameSize = GuiUtils.Graphics.MeasureString(TypedModel.Name, Font);
            model.Width = (int) Math.Max (typeSize.Width, nameSize.Width) + 5;
            model.Height = (int) (typeSize.Height + nameSize.Height) + 5;
        }

        /// <summary>
        /// When clicking on an element, select the wrapped model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        public override void HandleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            IModelElement model = TypedModel.Namable as IModelElement;
            if (model != null)
            {
                EfsSystem.Instance.Context.SelectElement(model, Panel, Context.SelectionCriteria.LeftClick);                
            }
        }

        /// <summary>
        /// The type of the element
        /// </summary>
        private string TypeName
        {
            get
            {
                string retVal = "";

                Type type = TypedModel.Namable.GetType();
                if (type != null)
                {
                    retVal = type.Name;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Draws the box in the panel
        /// </summary>
        /// <param name="g"></param>
        public override void PaintInBoxArrowPanel(Graphics g)
        {
            // Write the title
            string typeName = GuiUtils.AdjustForDisplay(TypeName, Width - 4, Bold);
            Brush textBrush = new SolidBrush(Color.Black);
            g.DrawString(typeName, Bold, textBrush, Location.X + 2, Location.Y + 2);
            g.DrawLine(NormalPen, new Point(Location.X, Location.Y + Font.Height + 2),
                new Point(Location.X + Width, Location.Y + Font.Height + 2));

            // Write the text in the box
            // Center the element name
            string name = GuiUtils.AdjustForDisplay(TypedModel.Name, Width, Font);
            SizeF textSize = g.MeasureString(name, Font);
            int boxHeight = Height - Bold.Height - 4;
            g.DrawString(name, Font, textBrush, Location.X + Width / 2 - textSize.Width / 2,
                Location.Y + Bold.Height + 4 + boxHeight / 2 - Font.Height / 2);
        }
    }
}
