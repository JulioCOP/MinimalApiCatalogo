namespace MinimalAPICatalogo.AppServicesExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        } //tratamento de exceção
        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(p =>
            {
                p.AllowAnyOrigin();
                p.WithMethods("Get");
                p.AllowAnyHeader();
            }); // Habilitando o CORS
            return app;
        }
        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { });
            return app;
        } // Habilitadno o Middleware do Swagger
    }
}
