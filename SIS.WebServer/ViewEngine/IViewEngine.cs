﻿namespace SIS.MvcFramework.ViewEngine
{
    public interface IViewEngine
    {
        string GetHtml<T>(string viewContent, T model);
    }
}
