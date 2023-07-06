using UnityEngine;
using VContainer.Unity;

namespace CodeBase.Infrastructure.Di
{
    public sealed class SceneScopeProvider
    {
        public LifetimeScope Scope()
            => Object.FindObjectOfType<LifetimeScope>();

        public LifetimeScope Parent()
            => Object.FindObjectOfType<LifetimeScope>().Parent;
    }
}