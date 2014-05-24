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
        [WebGet(UriTemplate = "GetConverter/{LHDiam}/{LHDiamDesc}/{RHDiam}/{RHDiamDesc}/{Magnification}/picture.png")]
        Stream GetConverterPicture(string LHDiam, string LHDiamDesc, string RHDiam, string RHDiamDesc, string Magnification);

        [Description("Get Picture Of Shaft Coupler")]
        [WebGet(UriTemplate = "GetCoupler/{LHDiam}/{LHDiamDesc}/{RHDiam}/{RHDiamDesc}/{Magnification}/{NumberOfGrubScrews}/{GrubScrewColour}/picture.png")]
        Stream GetCouplerPicture(string LHDiam, string LHDiamDesc, string RHDiam, string RHDiamDesc, string Magnification, string NumberOfGrubScrews, string GrubScrewColour);
    }
}
