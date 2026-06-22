using Bindito.Core;
using Timberborn.Automation;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Calloatti.DepthMonitor
{
  [Context("Game")]
  internal class DepthMonitorConfigurator : Configurator
  {
    private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
    {
      private readonly DepthMonitorFragment _fragment;

      public EntityPanelModuleProvider(DepthMonitorFragment fragment)
      {
        _fragment = fragment;
      }

      public EntityPanelModule Get()
      {
        EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
        builder.AddMiddleFragment(_fragment);
        return builder.Build();
      }
    }

    protected override void Configure()
    {
      Bind<DepthMonitor>().AsTransient();
      Bind<DepthMonitorMarker>().AsTransient();
      Bind<DepthMonitorFragment>().AsSingleton();

      MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
      MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
    }

    private static TemplateModule ProvideTemplateModule()
    {
      TemplateModule.Builder builder = new TemplateModule.Builder();

      // Critical attachments matching your vanilla layout
      builder.AddDecorator<DepthMonitorSpec, DepthMonitor>();
      builder.AddDecorator<DepthMonitor, Automator>();
      builder.AddDecorator<DepthMonitor, AutomatorIlluminator>();
      builder.AddDecorator<DepthMonitor, DepthMonitorMarker>();

      return builder.Build();
    }
  }
}