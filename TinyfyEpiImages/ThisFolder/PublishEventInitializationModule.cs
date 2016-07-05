using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace TinyfyEpiImages
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PublishEventInitializationModule : IInitializableModule
    {
        private Injected<IContentEvents> _injectedContentEvents;
        public void Initialize(InitializationEngine context)
        {
            _injectedContentEvents.Service.PublishedContent += ContentEvents.PublishedContent;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            _injectedContentEvents.Service.PublishedContent += ContentEvents.PublishedContent;
        }
    }
}
