using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    [StructLayout(LayoutKind.Sequential, Size = 10)]
    public struct G2FIXED_RESERVED_10_SBYTE
    {
        public sbyte r0;
        public sbyte r1;
        public sbyte r2;
        public sbyte r3;
        public sbyte r4;
        public sbyte r5;
        public sbyte r6;
        public sbyte r7;
        public sbyte r8;
        public sbyte r9;
    }

    [StructLayout(LayoutKind.Sequential, Size = 2)]
    public struct G2FIXED_BUF_SBYTE_2
    {
        public sbyte s0;
        public sbyte s1;
    }

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct G2FIXED_BUF_8
    {
        public byte b0;
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte b6;
        public byte b7;

        public void from(byte[] buf)
        {
            int len = Math.Min(Marshal.SizeOf(typeof(G2FIXED_BUF_8)), buf.Length);
            G2FIXED_BUF_8[] dst = new G2FIXED_BUF_8[1];
            Buffer.BlockCopy(buf, 0, dst, 0, len);
            this = dst[0];
        }
        public byte[] to()
        {
            return new byte[] { b0, b1, b2, b3, b4, b5, b6, b7 };
        }
        public void serialize(BinaryWriter s)
        {
            s.Write(b0);
            s.Write(b1);
            s.Write(b2);
            s.Write(b3);
            s.Write(b4);
            s.Write(b5);
            s.Write(b6);
            s.Write(b7);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 256)]
    public struct G2FIXED_BUF_256
    {
        public byte b0;
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte b6;
        public byte b7;
        public byte b8;
        public byte b9;
        public byte b10;
        public byte b11;
        public byte b12;
        public byte b13;
        public byte b14;
        public byte b15;
        public byte b16;
        public byte b17;
        public byte b18;
        public byte b19;
        public byte b20;
        public byte b21;
        public byte b22;
        public byte b23;
        public byte b24;
        public byte b25;
        public byte b26;
        public byte b27;
        public byte b28;
        public byte b29;
        public byte b30;
        public byte b31;
        public byte b32;
        public byte b33;
        public byte b34;
        public byte b35;
        public byte b36;
        public byte b37;
        public byte b38;
        public byte b39;
        public byte b40;
        public byte b41;
        public byte b42;
        public byte b43;
        public byte b44;
        public byte b45;
        public byte b46;
        public byte b47;
        public byte b48;
        public byte b49;
        public byte b50;
        public byte b51;
        public byte b52;
        public byte b53;
        public byte b54;
        public byte b55;
        public byte b56;
        public byte b57;
        public byte b58;
        public byte b59;
        public byte b60;
        public byte b61;
        public byte b62;
        public byte b63;
        public byte b64;
        public byte b65;
        public byte b66;
        public byte b67;
        public byte b68;
        public byte b69;
        public byte b70;
        public byte b71;
        public byte b72;
        public byte b73;
        public byte b74;
        public byte b75;
        public byte b76;
        public byte b77;
        public byte b78;
        public byte b79;
        public byte b80;
        public byte b81;
        public byte b82;
        public byte b83;
        public byte b84;
        public byte b85;
        public byte b86;
        public byte b87;
        public byte b88;
        public byte b89;
        public byte b90;
        public byte b91;
        public byte b92;
        public byte b93;
        public byte b94;
        public byte b95;
        public byte b96;
        public byte b97;
        public byte b98;
        public byte b99;
        public byte b100;
        public byte b101;
        public byte b102;
        public byte b103;
        public byte b104;
        public byte b105;
        public byte b106;
        public byte b107;
        public byte b108;
        public byte b109;
        public byte b110;
        public byte b111;
        public byte b112;
        public byte b113;
        public byte b114;
        public byte b115;
        public byte b116;
        public byte b117;
        public byte b118;
        public byte b119;
        public byte b120;
        public byte b121;
        public byte b122;
        public byte b123;
        public byte b124;
        public byte b125;
        public byte b126;
        public byte b127;
        public byte b128;
        public byte b129;
        public byte b130;
        public byte b131;
        public byte b132;
        public byte b133;
        public byte b134;
        public byte b135;
        public byte b136;
        public byte b137;
        public byte b138;
        public byte b139;
        public byte b140;
        public byte b141;
        public byte b142;
        public byte b143;
        public byte b144;
        public byte b145;
        public byte b146;
        public byte b147;
        public byte b148;
        public byte b149;
        public byte b150;
        public byte b151;
        public byte b152;
        public byte b153;
        public byte b154;
        public byte b155;
        public byte b156;
        public byte b157;
        public byte b158;
        public byte b159;
        public byte b160;
        public byte b161;
        public byte b162;
        public byte b163;
        public byte b164;
        public byte b165;
        public byte b166;
        public byte b167;
        public byte b168;
        public byte b169;
        public byte b170;
        public byte b171;
        public byte b172;
        public byte b173;
        public byte b174;
        public byte b175;
        public byte b176;
        public byte b177;
        public byte b178;
        public byte b179;
        public byte b180;
        public byte b181;
        public byte b182;
        public byte b183;
        public byte b184;
        public byte b185;
        public byte b186;
        public byte b187;
        public byte b188;
        public byte b189;
        public byte b190;
        public byte b191;
        public byte b192;
        public byte b193;
        public byte b194;
        public byte b195;
        public byte b196;
        public byte b197;
        public byte b198;
        public byte b199;
        public byte b200;
        public byte b201;
        public byte b202;
        public byte b203;
        public byte b204;
        public byte b205;
        public byte b206;
        public byte b207;
        public byte b208;
        public byte b209;
        public byte b210;
        public byte b211;
        public byte b212;
        public byte b213;
        public byte b214;
        public byte b215;
        public byte b216;
        public byte b217;
        public byte b218;
        public byte b219;
        public byte b220;
        public byte b221;
        public byte b222;
        public byte b223;
        public byte b224;
        public byte b225;
        public byte b226;
        public byte b227;
        public byte b228;
        public byte b229;
        public byte b230;
        public byte b231;
        public byte b232;
        public byte b233;
        public byte b234;
        public byte b235;
        public byte b236;
        public byte b237;
        public byte b238;
        public byte b239;
        public byte b240;
        public byte b241;
        public byte b242;
        public byte b243;
        public byte b244;
        public byte b245;
        public byte b246;
        public byte b247;
        public byte b248;
        public byte b249;
        public byte b250;
        public byte b251;
        public byte b252;
        public byte b253;
        public byte b254;
        public byte b255;

        public void from(byte[] buf)
        {
            int len = Math.Min(Marshal.SizeOf(typeof(G2FIXED_BUF_256)), buf.Length);
            G2FIXED_BUF_256[] dst = new G2FIXED_BUF_256[1];
            Buffer.BlockCopy(buf, 0, dst, 0, len);
            this = dst[0];
        }
        public byte[] to()
        {
            return new byte[] {
                b0,
                b1,
                b2,
                b3,
                b4,
                b5,
                b6,
                b7,
                b8,
                b9,
                b10,
                b11,
                b12,
                b13,
                b14,
                b15,
                b16,
                b17,
                b18,
                b19,
                b20,
                b21,
                b22,
                b23,
                b24,
                b25,
                b26,
                b27,
                b28,
                b29,
                b30,
                b31,
                b32,
                b33,
                b34,
                b35,
                b36,
                b37,
                b38,
                b39,
                b40,
                b41,
                b42,
                b43,
                b44,
                b45,
                b46,
                b47,
                b48,
                b49,
                b50,
                b51,
                b52,
                b53,
                b54,
                b55,
                b56,
                b57,
                b58,
                b59,
                b60,
                b61,
                b62,
                b63,
                b64,
                b65,
                b66,
                b67,
                b68,
                b69,
                b70,
                b71,
                b72,
                b73,
                b74,
                b75,
                b76,
                b77,
                b78,
                b79,
                b80,
                b81,
                b82,
                b83,
                b84,
                b85,
                b86,
                b87,
                b88,
                b89,
                b90,
                b91,
                b92,
                b93,
                b94,
                b95,
                b96,
                b97,
                b98,
                b99,
                b100,
                b101,
                b102,
                b103,
                b104,
                b105,
                b106,
                b107,
                b108,
                b109,
                b110,
                b111,
                b112,
                b113,
                b114,
                b115,
                b116,
                b117,
                b118,
                b119,
                b120,
                b121,
                b122,
                b123,
                b124,
                b125,
                b126,
                b127,
                b128,
                b129,
                b130,
                b131,
                b132,
                b133,
                b134,
                b135,
                b136,
                b137,
                b138,
                b139,
                b140,
                b141,
                b142,
                b143,
                b144,
                b145,
                b146,
                b147,
                b148,
                b149,
                b150,
                b151,
                b152,
                b153,
                b154,
                b155,
                b156,
                b157,
                b158,
                b159,
                b160,
                b161,
                b162,
                b163,
                b164,
                b165,
                b166,
                b167,
                b168,
                b169,
                b170,
                b171,
                b172,
                b173,
                b174,
                b175,
                b176,
                b177,
                b178,
                b179,
                b180,
                b181,
                b182,
                b183,
                b184,
                b185,
                b186,
                b187,
                b188,
                b189,
                b190,
                b191,
                b192,
                b193,
                b194,
                b195,
                b196,
                b197,
                b198,
                b199,
                b200,
                b201,
                b202,
                b203,
                b204,
                b205,
                b206,
                b207,
                b208,
                b209,
                b210,
                b211,
                b212,
                b213,
                b214,
                b215,
                b216,
                b217,
                b218,
                b219,
                b220,
                b221,
                b222,
                b223,
                b224,
                b225,
                b226,
                b227,
                b228,
                b229,
                b230,
                b231,
                b232,
                b233,
                b234,
                b235,
                b236,
                b237,
                b238,
                b239,
                b240,
                b241,
                b242,
                b243,
                b244,
                b245,
                b246,
                b247,
                b248,
                b249,
                b250,
                b251,
                b252,
                b253,
                b254,
                b255
            };
        }
    }
}
