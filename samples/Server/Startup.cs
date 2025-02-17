using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Models;
using SoapCore;

namespace Server
{
	public class Startup
	{
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
		{
			services.AddSoapCore();
			services.TryAddSingleton<ISampleService, SampleService>();
			services.AddMvc();
			services.AddReverseProxy().LoadFromConfig(Configuration.GetSection("ReverseProxy"));
		}

		public void Configure(IApplicationBuilder app)
		{			
			app.UseRouting();

			app.UseEndpoints(endpoints => {
				endpoints.UseSoapEndpoint<ISampleService>("/Service.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
				endpoints.UseSoapEndpoint<ISampleService>("/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
				endpoints.MapReverseProxy();
			});
		}
	}
}
