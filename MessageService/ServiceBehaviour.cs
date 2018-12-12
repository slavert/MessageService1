using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace MessageService
{
    //Service behavior
    public class ServiceBehaviorAttribute : Attribute, IServiceBehavior
    {

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = new InternalErrorHandler();

            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }

    }
}