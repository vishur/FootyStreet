using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace IndianFootyShop
{
    public static class UnityContainerBootstrapper
    {
        public static IUnityContainer Bootstrap()
        {

            IUnityContainer ContainerUnity = new UnityContainer();
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.UnityConfiguration) };

            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
            ContainerUnity.LoadConfiguration(unitySection, Constants.UnityContainerName);
            return ContainerUnity;
        }
    }
}