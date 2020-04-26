// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - MentorRequest.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum MentorRequest : ushort
    {
        REQUEST_APPRENTICE = 1,
        REQUEST_MENTOR = 2,
        LEAVE_MENTOR = 3,
        EXPELL_APPRENTICE = 4,
        ACCEPT_REQUEST_APPRENTICE = 8,
        ACCEPT_REQUEST_MENTOR = 9,
        DUMP_APPRENTICE = 18,
        DUMP_MENTOR = 19
    }
}