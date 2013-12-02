using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class DocTitleShadow : Shadow {
        public DocTitleShadow(Shape shape)
            : base(shape) {
            // apparently there are shapes here which used to display the title information
            // we could delete these in an upgrade (don't forget the vss file) if we want
            shape.Shapes[1].Text = string.Empty;
            shape.Shapes[2].Text = string.Empty;
            shape.Shapes[3].Text = string.Empty;
            shape.Shapes[4].Text = string.Empty;
            shape.Shapes[5].Text = string.Empty;
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            DocTitleForm form = new DocTitleForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal string GetClientName() {
            return Common.GetCellString(shape, ShapeProperties.DocTitle.ClientName);
        }

        internal string GetProjectName() {
            return Common.GetCellString(shape, ShapeProperties.DocTitle.ProjectName);
        }

        internal string GetLogoData() {
            return Common.GetCellString(shape, ShapeProperties.DocTitle.LogoData);
        }

        internal void SetClientName(string clientName) {
            Common.SetCellString(shape, ShapeProperties.DocTitle.ClientName, clientName);
        }

        internal void SetProjectName(string projectName) {
            Common.SetCellString(shape, ShapeProperties.DocTitle.ProjectName, projectName);
        }

        internal void SetLogoData(string logoData) {
            Common.SetCellString(shape, ShapeProperties.DocTitle.LogoData, logoData);
        }
    }
}
