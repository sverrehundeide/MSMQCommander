using System.ComponentModel;
using System.Windows;
using Autofac;
using MSMQCommander.Contex;
using MsmqLib;

namespace MSMQCommander
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Caliburn.Micro;

	public class AppBootstrapper : Bootstrapper<IShell>
	{
	    private Autofac.IContainer _container;

		protected override void Configure()
		{
		    var builder = new ContainerBuilder();

		    builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
		        .Where(type => type.Name.EndsWith("ViewModel"))
		        .Where(type => type.GetInterface(typeof (INotifyPropertyChanged).Name) != null)
		        .AsSelf()
		        .InstancePerDependency();

		    builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
		        .Where(type => type.Name.EndsWith("View"))
		        .AsSelf()
		        .InstancePerDependency();

            builder.RegisterType<ShellViewModel>().As<IShell>();

		    builder.Register<IWindowManager>(c => new WindowManager())
		        .InstancePerLifetimeScope();

		    builder.Register<IEventAggregator>(c => new EventAggregator())
		        .InstancePerLifetimeScope();

		    builder.RegisterType<CurrentSelectedQueueContext>()
		        .AsSelf()
		        .InstancePerLifetimeScope();

		    builder.RegisterType<QueueService>().As<IQueueService>();

		    builder.Register(c => new QueueConnectionContext {ComputerName = "."})
		        .AsSelf()
		        .InstancePerLifetimeScope();

		    _container = builder.Build();
		}

        protected override object GetInstance(Type serviceType, string key)
		{
            if (string.IsNullOrWhiteSpace(key))
            {
                if (_container.IsRegistered(serviceType))
                    return _container.Resolve(serviceType);
            }
            else
            {
                var type = Type.GetType(key);
                if (_container.IsRegistered(type))
                    return _container.Resolve(type);
            }
            throw new Exception(string.Format("Could not locate any instances for type {0}.", serviceType));
	    }

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
		    var instances = _container.Resolve(typeof (IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
		    return instances;
		}

		protected override void BuildUp(object instance)
		{
		    _container.InjectProperties(instance);
		}

        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO: Logging + new view for showing exceptions
            MessageBox.Show(e.Exception.ToString(), "Exception in MSMQ Commander", MessageBoxButton.OK, MessageBoxImage.Error); 
            e.Handled = true;
        }
	}
}
