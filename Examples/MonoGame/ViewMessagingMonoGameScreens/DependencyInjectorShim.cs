// <copyright file="DependencyInjectorShim.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace ViewMessagingMonoGameScreens;

using System;
using Codefarts.DependencyInjection;
using Codefarts.IoC;

public class DependencyInjectorShim : IDependencyInjectionProvider
{
    private Container ioc;

    public DependencyInjectorShim(Container ioc)
    {
        this.ioc = ioc;
    }

    public void Register(Type key, Type concrete)
    {
        this.ioc.Register(key, concrete);
    }

    public void Register(Type key, Func<object> callback)
    {
        this.ioc.Register(key, () => callback());
    }
                                                                                
    public object Resolve(Type type)
    {
        return this.ioc.Resolve(type);
    }

    public void ResolveMembers(object value)
    {
        this.ioc.ResolveMembers(value);
    }
}