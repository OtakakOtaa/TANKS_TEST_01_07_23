using VContainer;

namespace CodeBase.Infrastructure.Di
{
    public abstract class ConfigRegisterScope<TParams>
    {
        protected readonly IContainerBuilder Builder;

        protected ConfigRegisterScope(IContainerBuilder builder)
            => Builder = builder;
        
        public abstract void Configure(TParams @params);
    }
}