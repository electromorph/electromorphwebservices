using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Configuration;
using System.ComponentModel;
using System.Text;
using System.ServiceModel.Activation;
using System.IO;

namespace emsvc
{
    [ServiceContract]
    public interface IService
    {
        [Description("Simple echo operation over HTTP GET")]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string EchoWithGet(string s);

        [Description("Get Picture Of Shaft Converter")]
        [WebGet(UriTemplate = "GetConverter/{IsLHDiamMetric}/{LHDiam}/{IsRHDiamMetric}/{RHDiam}/{Magnification}/picture.png")]
        Stream GetConverterPicture(string IsLHDiamMetric, string LHDiam, string IsRHDiamMetric, string RHDiam, string Magnification);

        [Description("Get Picture Of Shaft Coupler")]
        [WebGet(UriTemplate = "GetCoupler/{IsLHDiamMetric}/{LHDiam}/{IsRHDiamMetric}/{RHDiam}/{Magnification}/{NumberOfGrubScrews}/{GrubScrewColour}/picture.png")]
        Stream GetCouplerPicture(string IsLHDiamMetric, string LHDiam, string IsRHDiamMetric, string RHDiam, string Magnification, string NumberOfGrubScrews, string GrubScrewColour);

        [Description("Get Price of shaft coupler")]
        [WebGet(UriTemplate = "GetCouplerPrice/{IsLHDiamMetric}/{LHDiam}/{IsRHDiamMetric}/{RHDiam}/{Quantity}")]
        Stream GetCouplerPrice(string IsLHDiamMetric, string LHDiam, string IsRHDiamMetric, string RHDiam, string Quantity);
    }
}
