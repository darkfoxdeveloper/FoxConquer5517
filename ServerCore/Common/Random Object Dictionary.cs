// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Random Object Dictionary.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerCore.Common
{
    public static class RandomObjectDictionary
    {
        public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var rand = new Random();
            List<TValue> values = dict.Values.ToList();
            int size = values.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }
}