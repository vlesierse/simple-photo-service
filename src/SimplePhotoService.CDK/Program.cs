using Constructs;
using SimplePhotoService.CDK;

var app = new App();

var environmentName = app.GetEnvironmentName();
Tags.Of(app).Add("application", "SimplePhotoService");
Tags.Of(app).Add("environment", environmentName);

var _ = new ApplicationStack(app, $"SimplePhotoService-Application-{environmentName}", new ApplicationStackProps());

app.Synth();
