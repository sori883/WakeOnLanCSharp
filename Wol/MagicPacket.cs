﻿using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Wol
{
    /// <summary>
    /// MACからマジックパケットを取得し、送信する。
    /// </summary>
    class MagicPacket
    {
        //ブロードキャストアドレス
        private const String BROADCAST = "255.255.255.255";
        //ブロードキャストアドレス
        private const int PORT = 2304;

        //マックアドレス
        ArrayList macs = new ArrayList();

        public MagicPacket()
        {
            //マックを取得する処理

            byte[] phy;
            byte[] packet;
            for (int i = 0; i < macs.Count; i++)
            {
                phy = ParseMac((String)macs[i]);
                packet = CreateMagicPacket(phy);
                SendMagicPacket(packet);
            }
        }

        /// <summary>
        /// 文字列を分解して、MACのバイト列に変換して返します。
        /// </summary>
        /// <param name="mac">String型のMAC</param>
        /// <returns>byte型のMAC</returns>
        private byte[] ParseMac(String mac)
        {
            byte[] phy = new byte[6];
            string[] remove = mac.Split("-: ".ToCharArray());

            for (int i = 0; i < phy.Length; i++)
            {
                phy[i] = Convert.ToByte(remove[i], 16);
            }
            return phy;
        }

        /// <summary>
        /// MACよりマジックパケットを作成する
        /// </summary>
        /// <param name="phy">分解したMAC</param>
        /// <returns>マジックパケット</returns>
        private byte[] CreateMagicPacket(byte[] phy)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            for (int i = 0; i < 6; i++)
            {
                writer.Write((byte)0xff);
            }

            for (int i = 0; i < 16; i++)
            {
                writer.Write(phy);
            }

            byte[] packet = new byte[stream.Position];
            Array.Copy(stream.ToArray(), packet, packet.Length);

            return packet;
        }

        /// <summary>
        /// マジックパケットを送信する
        /// </summary>
        /// <param name="packet">マジックパケット</param>
        private void SendMagicPacket(byte[] packet)
        {
            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            client.Send(packet, packet.Length, new IPEndPoint(IPAddress.Parse(BROADCAST) , PORT));
        }
    }
}