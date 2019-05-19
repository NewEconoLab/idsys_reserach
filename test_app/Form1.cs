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

        private void Button1_Click(object sender, EventArgs e)
        {
            //neo 地址 和 ID 相关
            // 和密码无关

            //将私钥=》地址

            //变成ID=》地址

            //密码可以改变


            var idb= System.Text.Encoding.UTF8.GetBytes(text_id.Text);

            var hash256 = ThinNeo.Helper.Sha256(idb);
            hash256 = Helper.Sha256(hash256);

            list_info.Items.Add("id=" + Helper.Bytes2HexString(hash256));

            byte[] script = null;//生成特殊账户
            {
                ScriptBuilder sb = new ScriptBuilder();
                sb.EmitPushBytes(hash256);
                sb.EmitPushString("getkey_scripthash");
                sb.EmitAppCall(new Hash160("0xd29efab1d0d686588f5ac312348b5e9c65e3c829"));
                sb.Emit(ThinNeo.VM.OpCode.CHECKSIG);
                script = sb.ToArray();
            }

            var addrhash = Helper.GetScriptHashFromScript(script);
            var addr = Helper.GetAddressFromScriptHash(addrhash);
            list_info.Items.Add("script=" + Helper.Bytes2HexString(script));
            list_info.Items.Add("addr=" + addr);

        }
    }
}
