using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Text;
using System.IO;
using System.Net;

namespace emsvc
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1 : IService
    {
        public string EchoWithGet(string s)
        {
            return "You said " + s;
        }

        public bool IsYes(string Variable)
        {
            Variable = Variable.ToLower();
            return (Variable == "true");
        }

        public Stream GetCouplerPicture(string IsLHDiamMetric, string LHDiam, string IsRHDiamMetric, string RHDiam, string Magnification)
        {
            var scp = new ShaftCouplerPicture();
            double LHD, RHD;
            int magnification;
            if (!double.TryParse(LHDiam, out LHD) || !double.TryParse(RHDiam, out RHD) || !int.TryParse(Magnification, out magnification))
            {
                throw new WebFaultException<string>("The LHDiam or RHDiam fields were not of type 'double'.", HttpStatusCode.BadRequest);
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            return scp.GetCouplerPicture(IsYes(IsLHDiamMetric), LHD, IsYes(IsRHDiamMetric), RHD, magnification);
        }

        public Stream GetConverterPicture(string IsLHDiamMetric, string LHDiam, string IsRHDiamMetric, string RHDiam, string Magnification)
        {
            var scp = new ShaftConverterPicture();
            double LHD, RHD;
            int magnification;
            if (!double.TryParse(LHDiam, out LHD) || !double.TryParse(RHDiam, out RHD) || !int.TryParse(Magnification, out magnification))
            {
                throw new WebFaultException<string>("The LHDiam or RHDiam fields were not of type 'double'.", HttpStatusCode.BadRequest);
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            return scp.GetConverterPicture(IsYes(IsLHDiamMetric), LHD, IsYes(IsRHDiamMetric), RHD, magnification);
        }

        public Stream GetCouplerPrice(string isLHDimensionMetric, string LHDiam, string isRHDimensionMetric, string RHDiam, string quantity)
        {
            double LHD, RHD;
            int Quantity;
            if (!double.TryParse(LHDiam, out LHD) || !double.TryParse(RHDiam, out RHD) || !int.TryParse(quantity, out Quantity))
            {
                throw new WebFaultException<string>("The LHDiam or RHDiam fields were not of type 'double'.", HttpStatusCode.BadRequest);
            }
            //Convert from inches if necessary
            double leftHandDiameterInmm = IsYes(isLHDimensionMetric) ? LHD : (LHD * 25.4);
            double rightHandDiameterInmm = IsYes(isRHDimensionMetric) ? RHD : (RHD * 25.4);
            string leftHandDimensionText = IsYes(isLHDimensionMetric) ? LHD.ToString() + "mm" : Convert.ToInt32((LHD * 16)).ToString() + "/16\"";
            string rightHandDimensionText = IsYes(isRHDimensionMetric) ? RHD.ToString() + "mm" : Convert.ToInt32((RHD * 16)).ToString() + "/16\"";
            double largestDiameter = (LHD > RHD) ? LHD : RHD;
            double price = 0;
            if (Quantity == 1)
            {
                price = (largestDiameter <= 3) ? 3.99 : ((largestDiameter <= 10) ? 4.50 : ((largestDiameter <= 14) ? 4.99 : 5.50));
            }
            else if (Quantity >= 2 && Quantity <= 5)
            {
                price = (largestDiameter <= 3) ? 2.75 : ((largestDiameter <= 10) ? 3.25 : ((largestDiameter <= 14) ? 3.75 : 4.25));
            }
            else if (Quantity >= 6)
            {
                price = (largestDiameter <= 3) ? 1.99 : ((largestDiameter <= 10) ? 2.50 : ((largestDiameter <= 14) ? 2.99 : 3.25));
            }
            string result = price.ToString("#,#.00#");
            byte[] resultBytes = Encoding.UTF8.GetBytes(result);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return new MemoryStream(resultBytes);
        }
    }
}
