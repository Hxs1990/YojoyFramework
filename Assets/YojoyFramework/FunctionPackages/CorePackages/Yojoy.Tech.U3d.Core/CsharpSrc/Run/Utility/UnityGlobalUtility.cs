﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yojoy.Tech.U3d.Core.Run
{
    public static class UnityGlobalUtility 
    {
        public static string GetPrefsKey(string keyId,Type type)
        {
            var finalKeys = type.Name + "_" + keyId;
            return finalKeys;
        }
    }
}

