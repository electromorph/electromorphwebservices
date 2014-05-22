using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.UI;
using System.IO;
//using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace emsvc
{
    [Serializable]
    public class ShaftCouplerPicture
    {
        int leftHandDiameterInHalfmm = 0;
        int rightHandDiameterInHalfmm = 0;
        int topOffset = 20;  //This is to accommodate a dimension
        int bottomOffset = 30;  //This is to accommodate a dimension
        int leftOffset = 40;  //This is to accommodate a dimension
        int rightOffset = 100;  //Leaving space for a dimension at the bottom
        string Diam1 = string.Empty;
        string Diam2 = string.Empty;

        public MemoryStream GetCouplerPicture(bool isLHDimensionMetric, double leftHandDiameterPassedIn, bool isRHDimensionMetric, double rightHandDiameterPassedIn, int magnification, int numGrubScrews, string grubScrewColour)
        {
            //Convert from inches if necessary
            double leftHandDiameterInmm = isLHDimensionMetric ? leftHandDiameterPassedIn : (leftHandDiameterPassedIn * 25.4);
            double rightHandDiameterInmm = isRHDimensionMetric ? rightHandDiameterPassedIn : (rightHandDiameterPassedIn * 25.4);
            leftHandDiameterInHalfmm = Convert.ToInt32(2 * leftHandDiameterInmm);
            rightHandDiameterInHalfmm = Convert.ToInt32(2 * rightHandDiameterInmm);
            string leftHandDimensionText = isLHDimensionMetric ? leftHandDiameterPassedIn.ToString() + "mm" : Convert.ToInt32((leftHandDiameterPassedIn*16)).ToString() + "/16\"";
            string rightHandDimensionText = isRHDimensionMetric ? rightHandDiameterPassedIn.ToString() + "mm" : Convert.ToInt32((rightHandDiameterPassedIn * 16)).ToString() + "/16\"";
            Bitmap bmp = this.GenerateBitmap(leftHandDiameterInHalfmm, rightHandDiameterInHalfmm, magnification, leftHandDimensionText, rightHandDimensionText, numGrubScrews, grubScrewColour);
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return ms;
        }

        private Bitmap GenerateBitmap(double Diam1, double Diam2, int sizeMultiplier, string leftHandDimensionText, string rightHandDimensionText, int numGrubScrews, string grubScrewColour)
        {
            if ((sizeMultiplier <= 0) || (sizeMultiplier > 20))
            {
                sizeMultiplier = 4;
            }
            //Make it 8mm for all.
            int width = sizeMultiplier * 60;  //20 = 10mm (length) * 2 (because diam is already in half-mm)
            //GetLargestDiameter
            int largestDiameterInHalfmm = (leftHandDiameterInHalfmm > rightHandDiameterInHalfmm) ? leftHandDiameterInHalfmm : rightHandDiameterInHalfmm;
            int height = sizeMultiplier * ((largestDiameterInHalfmm <= 10) ? 16 : ((largestDiameterInHalfmm <= 14) ? 20 : 24));
            int leftHandDiameterAsDrawn = leftHandDiameterInHalfmm * sizeMultiplier;
            int rightHandDiameterAsDrawn = rightHandDiameterInHalfmm * sizeMultiplier;
            Bitmap bmp = new Bitmap(width + rightOffset + leftOffset, height + topOffset + bottomOffset);
            Graphics g = Graphics.FromImage(bmp);
            GraphicsPath whiteBackground = new GraphicsPath();
            whiteBackground.AddRectangle(new Rectangle(0, 0, width + rightOffset + leftOffset, height + topOffset + bottomOffset));
            g.FillPath(new SolidBrush(Color.White), whiteBackground);
            GraphicsPath shaftCoupler = DrawCouplerSide1(width, height, leftHandDiameterAsDrawn, rightHandDiameterAsDrawn);
            //g.FillPath(new SolidBrush(Color.Gold), shaftCoupler);
            var brush = new LinearGradientBrush(new Point(leftOffset,topOffset), new Point(leftOffset+width,topOffset), Color.Gold, Color.Goldenrod);
            SolidBrush solidBlack = new SolidBrush(Color.Black);
            SolidBrush grubScrewBrush = new SolidBrush((grubScrewColour == "silver" ? Color.Silver : Color.Black));
            g.FillPath(brush, shaftCoupler);
            System.Drawing.Pen pen = new Pen(Color.Black, 0);
            System.Drawing.Pen screwPen = new Pen((grubScrewColour == "silver" ? Color.Silver : Color.Black), 0);
            g.DrawPath(pen, shaftCoupler);
            GraphicsPath shaftCouplerSide2 = DrawCouplerSide2(width, height, leftHandDiameterAsDrawn, rightHandDiameterAsDrawn);
            g.FillPath(brush, shaftCouplerSide2);
            g.DrawPath(pen, shaftCouplerSide2);
            g.DrawPath(pen, GetHeightDimension(width, height));
            g.DrawPath(pen, GetWidthDimension(width, height));
            g.DrawPath(pen, GetScrewPositionDimension(sizeMultiplier));
            GraphicsPath screw1Path = GetScrew(10 * sizeMultiplier, height / 2);
            GraphicsPath screw2Path = GetScrew(width - (10 * sizeMultiplier), height / 2);
            g.DrawPath(screwPen, screw1Path);
            g.FillPath(grubScrewBrush, screw1Path);
            g.DrawPath(screwPen, screw2Path);
            g.FillPath(grubScrewBrush, screw2Path);
            g.DrawPath(pen, GetRHDiameterDimension(width, height, rightHandDiameterAsDrawn));
            g.DrawPath(pen, GetLHDiameterDimension(width, height, leftHandDiameterAsDrawn));
            //Draw height dimension figure
            Font fn = new Font("Arial", 10);
            //Draw height dimension.  2 * because it's not in ½mm unlike diameters!
            g.DrawString((height / (2 * sizeMultiplier)).ToString() + "mm", fn, solidBlack, new PointF(leftOffset + width + 60, height / 2 - 8));
            //Draw width dimension
            g.DrawString("15mm", fn, solidBlack, new PointF(leftOffset + width / 2, topOffset + height + 10));
            //Draw screw dimension
            g.DrawString("2.5mm", fn, solidBlack, new PointF(leftOffset + 5, 0));
            //Draw RH diameter dimension
            string rhDiameterString = rightHandDimensionText;
            g.DrawString(rhDiameterString, fn, solidBlack, new PointF(leftOffset + width + 6, 10 + height / 2));
            //Draw LH diameter dimension
            string lhDiameterString = leftHandDimensionText;
            g.DrawString(lhDiameterString, fn, solidBlack, new PointF(0, 10 + height / 2));
            pen.Dispose();
            g.Dispose();
            return bmp;
        }

        private GraphicsPath GetScrewPositionDimension(int sizeMultiplier)
        {
            int offsetFromTop = topOffset - 8;
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            pathLine.AddLine(new Point(leftOffset + 3, offsetFromTop), new Point(leftOffset + 0, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 0, 3 + offsetFromTop), new Point(leftOffset + 3, 6 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 0, 3 + offsetFromTop), new Point(leftOffset + 10 * sizeMultiplier, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 10 * sizeMultiplier - 3, offsetFromTop), new Point(leftOffset + 10 * sizeMultiplier, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 10 * sizeMultiplier, 3 + offsetFromTop), new Point(leftOffset + 10 * sizeMultiplier - 3, 6 + offsetFromTop));
            path.AddPath(pathLine, true);
            return path;
        }

        private GraphicsPath GetWidthDimension(int W, int H)
        {
            int offsetFromTop = topOffset + H + 2;
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            pathLine.AddLine(new Point(leftOffset + 3, offsetFromTop), new Point(leftOffset + 0, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 0, 3 + offsetFromTop), new Point(leftOffset + 3, 6 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + 0, 3 + offsetFromTop), new Point(leftOffset + W, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + W - 3, offsetFromTop), new Point(leftOffset + W, 3 + offsetFromTop));
            pathLine.AddLine(new Point(leftOffset + W, 3 + offsetFromTop), new Point(leftOffset + W - 3, 6 + offsetFromTop));
            path.AddPath(pathLine, true);
            return path;
        }

        private GraphicsPath GetHeightDimension(int W, int H)
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            pathLine.AddLine(new Point(leftOffset + W + 54, 3 + topOffset), new Point(leftOffset + W + 56, topOffset));
            pathLine.AddLine(new Point(leftOffset + W + 58, 3 + topOffset), new Point(leftOffset + W + 56, topOffset));
            pathLine.AddLine(new Point(leftOffset + W + 56, topOffset), new Point(leftOffset + W + 56, H + topOffset));
            pathLine.AddLine(new Point(leftOffset + W + 54, H - 3 + topOffset), new Point(leftOffset + W + 56, H + topOffset));
            pathLine.AddLine(new Point(leftOffset + W + 58, H - 3 + topOffset), new Point(leftOffset + W + 56, H + topOffset));
            path.AddPath(pathLine, true);
            return path;
        }

        private GraphicsPath GetRHDiameterDimension(int W, int H, int rightHandDiameterAsDrawn)
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            int dimensionYMin = (H - rightHandDiameterAsDrawn) / 2 + topOffset;
            int dimensionYMax = (H + rightHandDiameterAsDrawn) / 2 + topOffset;
            pathLine.AddLine(new Point(leftOffset + W, dimensionYMin + 3), new Point(leftOffset + W + 2, dimensionYMin));
            pathLine.AddLine(new Point(leftOffset + W + 4, dimensionYMin + 3), new Point(leftOffset + W + 2, dimensionYMin));
            pathLine.AddLine(new Point(leftOffset + W + 2, dimensionYMin), new Point(leftOffset + W + 2, dimensionYMax));
            pathLine.AddLine(new Point(leftOffset + W, dimensionYMax - 3), new Point(leftOffset + W + 2, dimensionYMax));
            pathLine.AddLine(new Point(leftOffset + W + 4, dimensionYMax - 3), new Point(leftOffset + W + 2, dimensionYMax));
            path.AddPath(pathLine, true);
            return path;
        }

        private GraphicsPath GetLHDiameterDimension(int W, int H, int leftHandDiameterAsDrawn)
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            int dimensionYMin = (H - leftHandDiameterAsDrawn) / 2 + topOffset;
            int dimensionYMax = (H + leftHandDiameterAsDrawn) / 2 + topOffset;
            pathLine.AddLine(new Point(leftOffset, dimensionYMin + 3), new Point(leftOffset + 2, dimensionYMin));
            pathLine.AddLine(new Point(leftOffset + 4, dimensionYMin + 3), new Point(leftOffset + 2, dimensionYMin));
            pathLine.AddLine(new Point(leftOffset + 2, dimensionYMin), new Point(leftOffset + 2, dimensionYMax));
            pathLine.AddLine(new Point(leftOffset, dimensionYMax - 3), new Point(leftOffset + 2, dimensionYMax));
            pathLine.AddLine(new Point(leftOffset + 4, dimensionYMax - 3), new Point(leftOffset + 2, dimensionYMax));
            path.AddPath(pathLine, true);
            return path;
        }


        private GraphicsPath GetScrew(int xPosition, int length)
        {
            xPosition += leftOffset;
            int pitch = 2;
            length = length / pitch;
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            int i = 0;
            //Draw LHS of screw
            for (i = 0; i < length; i = i + 2)
            {
                pathLine.AddLine(new Point(xPosition - 3, i * pitch + topOffset-5), new Point(xPosition - 6, (i + 1) * pitch + topOffset-5));
                pathLine.AddLine(new Point(xPosition - 6, (i + 1) * pitch + topOffset-5), new Point(xPosition - 3, (i + 2) * pitch + topOffset-5));
            }
            //Draw Flat Bottom of screw
            pathLine.AddLine(new Point(xPosition - 3, i * pitch + topOffset), new Point(xPosition + 3, i * pitch + topOffset));
            //Draw RHS of screw
            for (i = length; i > 0; i = i - 2)
            {
                pathLine.AddLine(new Point(xPosition + 3, (i) * pitch + topOffset-5), new Point(xPosition + 6, (i - 1) * pitch + topOffset-5));
                pathLine.AddLine(new Point(xPosition + 6, (i - 1) * pitch + topOffset-5), new Point(xPosition + 3, (i - 2) * pitch + topOffset-5));
            }
            path.AddPath(pathLine, true);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath DrawCouplerSide1(int W, int H, int D1, int D2)
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            pathLine.AddLine(new Point(leftOffset, 0 + topOffset), new Point(leftOffset + W, 0 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W, 0 + topOffset), new Point(leftOffset + W, H / 2 - D2 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W, H / 2 - D2 / 2 + topOffset), new Point(leftOffset + W / 2, H / 2 - D2 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W / 2, H / 2 - D2 / 2 + topOffset), new Point(leftOffset + W / 2, H / 2 - D1 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W / 2, H / 2 - D1 / 2 + topOffset), new Point(leftOffset, H / 2 - D1 / 2 + topOffset));
            path.AddPath(pathLine, true);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath DrawCouplerSide2(int W, int H, int D1, int D2)
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath pathLine = new GraphicsPath();
            pathLine.AddLine(new Point(leftOffset, H + topOffset), new Point(leftOffset + W, H + topOffset));
            pathLine.AddLine(new Point(leftOffset + W, H + topOffset), new Point(leftOffset + W, H / 2 + D2 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W, H / 2 + D2 / 2 + topOffset), new Point(leftOffset + W / 2, H / 2 + D2 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W / 2, H / 2 + D2 / 2 + topOffset), new Point(leftOffset + W / 2, H / 2 + D1 / 2 + topOffset));
            pathLine.AddLine(new Point(leftOffset + W / 2, H / 2 + D1 / 2 + topOffset), new Point(leftOffset, H / 2 + D1 / 2 + topOffset));
            path.AddPath(pathLine, true);
            path.CloseFigure();
            return path;
        }
    }
}