// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - INpc.cs
// Last Edit: 2016/12/05 10:51
// Created: 2016/12/05 10:51
namespace MsgServer.Structures.Interfaces
{
    public interface INpc
    {
        uint Task0 { get; }
        uint Task1 { get; }
        uint Task2 { get; }
        uint Task3 { get; }
        uint Task4 { get; }
        uint Task5 { get; }
        uint Task6 { get; }
        uint Task7 { get; }
        int Data0 { get; set; }
        int Data1 { get; set; }
        int Data2 { get; set; }
        int Data3 { get; set; }
    }
}