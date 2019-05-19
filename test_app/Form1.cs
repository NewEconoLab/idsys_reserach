using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThinNeo;

namespace test_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[] genkey()
        {
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                BigInteger k;
                do
                {
                    k = rng.NextBigInteger(8 * 32);
                }
                while (k.Sign == 0 || k.CompareTo(ThinNeo.Cryptography.ECC.ECCurve.Secp256r1.N) >= 0);
                return k.ToByteArray().Take(32).ToArray();
            }
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            var prikey = genkey();
            var wif = ThinNeo.Helper.GetWifFromPrivateKey(prikey);
            this.text_wif.Text = wif;
        }
    }
}
