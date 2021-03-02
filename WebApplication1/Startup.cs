using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOData();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                IHttpResponseBodyFeature f = ctx.Features.Get<IHttpResponseBodyFeature>();

                f?.DisableBuffering();

                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapODataRoute("test", "test", GetModel());
            });
        }

        private IEdmModel GetModel()
        {
            EdmModel model = new EdmModel();

            // Complex Type
            EdmComplexType address = new EdmComplexType("WebApplication1", "Address");
            address.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);
            address.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(address);

            // Entity type
            EdmEntityType customer = new EdmEntityType("WebApplication1", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32));
            customer.AddStructuralProperty("Location", new EdmComplexTypeReference(address, isNullable: true));
            customer.AddStructuralProperty("Data", EdmPrimitiveTypeKind.Binary);
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("WebApplication1", "Container");
            EdmEntitySet customers = container.AddEntitySet("Customers", customer);
            model.AddElement(container);

            return model;
        }
    }
}
