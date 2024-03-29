﻿using System;
using Newtonsoft.Json;

namespace MinerFusionConsole.BuildingBlocks
{
    public static class JsonUtils
    {
        public static object ExtractJsonObject(string mixedString)
        {
            for (var i = mixedString.IndexOf('{'); i > -1; i = mixedString.IndexOf('{', i + 1))
            {
                for (var j = mixedString.LastIndexOf('}'); j > -1; j = mixedString.LastIndexOf("}", j - 1))
                {
                    var jsonProbe = mixedString.Substring(i, j - i + 1);
                    try
                    {
                        return JsonConvert.DeserializeObject(jsonProbe);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            return null;
        }
    }
}
