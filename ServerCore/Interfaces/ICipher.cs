// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - ICipher.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52
namespace ServerCore.Interfaces
{
    /// <summary>
    /// This interface is used to define a cipher for packet processing. The socket systems use this interface to
    /// decrypt the header and body of packets. All packet ciphers should implement this method.
    /// </summary>
    public interface ICipher
    {
        byte[] Decrypt(byte[] buffer, int length);
        void Decrypt(byte[] packet, byte[] buffer, int length, int position);
        byte[] Encrypt(byte[] packet, int length);
        void GenerateKeys(int account, int authentication);
        void KeySchedule(byte[] key);
    }
}