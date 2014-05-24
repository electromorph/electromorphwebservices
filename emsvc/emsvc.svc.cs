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

        public bool IsMetric(string Variable)
        {
            Variable = Variable.ToLower();
            return (Variable == "metric");
        }

        public Stream GetCouplerPicture(string LHDiam, string LHDiamDesc, string RHDiam, string RHDiamDesc, string Magnification, string NumberOfGrubScrews, string GrubScrewColour)
        {
            var scp = new ShaftCouplerPicture();
            int numGrubScrews = 0;
            double LHD, RHD;
            int magnification;
            if (!int.TryParse(NumberOfGrubScrews, out numGrubScrews))
            {
                throw new WebFaultException<string>("Number Of Grub Screws must be a number", HttpStatusCode.BadRequest);
            }
            if (!double.TryParse(LHDiam, out LHD) || !double.TryParse(RHDiam, out RHD) || !int.TryParse(Magnification, out magnification))
            {
                throw new WebFaultException<string>("The LHDiam or RHDiam fields were not of type 'double'.", HttpStatusCode.BadRequest);
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            return scp.GetCouplerPicture(LHD, LHDiamDesc, RHD, RHDiamDesc, magnification, numGrubScrews, GrubScrewColour);
        }

        public Stream GetConverterPicture(string LHDiam, string LHDiamDesc, string RHDiam, string RHDiamDesc, string Magnification)
        {
            var scp = new ShaftConverterPicture();
            double LHD, RHD;
            int magnification;
            if (!double.TryParse(LHDiam, out LHD) || !double.TryParse(RHDiam, out RHD) || !int.TryParse(Magnification, out magnification))
            {
                throw new WebFaultException<string>("The LHDiam or RHDiam fields were not of type 'double'.", HttpStatusCode.BadRequest);
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            return scp.GetConverterPicture(LHD, LHDiamDesc, RHD, RHDiamDesc, magnification);
        }
    }
}
