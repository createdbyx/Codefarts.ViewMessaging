// <copyright file="MonoGameScreenView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using BasicGameScreens;
using Codefarts.ViewMessaging;

namespace Codefarts.ViewMessaging.MonoGame.GameScreens;

using System;

/// <summary>
/// Provides a <see cref="IView"/> implementation for a Wpf UI element.
/// </summary>
/// <seealso cref="Codefarts.ViewMessaging.IView" />
public class MonoGameScreenView : IView
{
    private GameScreen screenReference;

    /// <inheritdoc />
    public string ViewName { get; }

    public ViewArguments Arguments { get; set; }

    /// <inheritdoc />
    public string Id { get; }

    public IViewService ViewService { get; }

    /// <inheritdoc />
    public object ViewReference
    {
        get
        {
            return this.screenReference;
        }
    }

    public MonoGameScreenView(IViewService viewService, GameScreen screen, string viewName)
    {
        if (viewName == null)
        {
            throw new ArgumentNullException(nameof(viewName));
        }

        if (string.IsNullOrWhiteSpace(viewName))
        {
            throw new ArgumentException("No view name specified.", nameof(viewName));
        }

        this.ViewService = viewService ?? throw new ArgumentNullException(nameof(viewService));

        this.Id = Guid.NewGuid().ToString();
        if (screen != null)
        {
            this.screenReference = screen;
        }

        this.ViewName = viewName;
    }

    public MonoGameScreenView(IViewService viewService, GameScreen screen, string viewName, ViewArguments args)
        : this(viewService, screen, viewName)
    {
        this.Arguments = args;
    }
}