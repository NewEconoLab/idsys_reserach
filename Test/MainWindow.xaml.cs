using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using ThinNeo;
using sdk;
using System.Numerics;
using System.Security.Cryptography;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Acc_ScriptPackage acc_ScriptPackage;
        private Wallet wallet;
        public MainWindow()
        {
            InitializeComponent();
            wallet = new Wallet("", this.tb_api.Text);
            acc_ScriptPackage = new Acc_ScriptPackage(this.tb_ContractHash.Text);
            //获取高度
            Task task = new Task(() => {
                UpdateInfo();
            });
            task.Start();
        }

        long height = 0;
        private void UpdateInfo()
        {
            while (true)
            {
                var _height = wallet.nelApiHelper.GetBlockCount() - 1;
                if (_height != height)
                {
                    Dispatcher.Invoke((Action)delegate () {
                        this.lb_blockHeight.Content = _height.ToString();
                        height = _height;
                    });

                }
                System.Threading.Thread.Sleep(5000);
            }
        }


        private void SignUp(object sender, RoutedEventArgs e)
        {
            try
            {
                // 使用一个IntPtr类型值来存储加密字符串的起始点  
                IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.wif.SecurePassword);

                // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
                string wif = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);

                wallet = new Wallet(wif, this.tb_api.Text);

                this.address.Content = wallet.Address;
            }
            catch
            {
                MessageBox.Show("请输入正确的wif");
            }
        }


        private void Invoke(byte[] script)
        {
            JObject result = wallet.nelApiHelper.InvokeScript(Helper.Bytes2HexString(script));

            details.Text = result.ToString();
        }

        private string MakeTran(byte[] script)
        {
            //如果没有wif不能发送交易
            if (wallet.PriKey == null)
            {
                MessageBox.Show("请先登入wif");
                return "";
            }

            if ((bool)multSign.IsChecked)//是多签
            {
                return "";
            }
            else
            {
                Transaction tran = wallet.MakeTran(script, int.Parse(tb_systemfee.Text), int.Parse(tb_netfee.Text));
                Console.WriteLine(Helper.Bytes2HexString(tran.GetRawData()));
                JObject result = wallet.nelApiHelper.SendRawTransaction(Helper.Bytes2HexString(tran.GetRawData()));
                //根据结果做出不同的操作
                return AnalysisResult(result);
            }
        }

        private string AnalysisResult(JObject result)
        {
            details.Text = result.ToString();
            if ((bool)result["result"][0]["sendrawtransactionresult"])
            {//交易发送成功
                string txid = result["result"][0]["txid"].ToString();
                //把txid加入交易详情
                Label lb = new Label();
                lb.Content = txid;
                lb.MouseDown += GetNotifyOrTransaction;
                this.txs.Items.Add(lb);
                return txid;
            }
            else
            {//交易发送失败
                return "";
            }
        }

        private void GetNotifyOrTransaction(object sender, RoutedEventArgs e)
        {
            Label lb = sender as Label;
            if (lb == null)
                return;
            var txid = lb.Content.ToString();
            JObject result = wallet.nelApiHelper.GetNotify(txid);
            if (result.ContainsKey("error"))
                result = wallet.nelApiHelper.Getrawtransaction(txid);
            this.details.Text = result.ToString();
        }

        private void UpdateScriptHash(object sender, RoutedEventArgs e)
        {
            acc_ScriptPackage = new Acc_ScriptPackage(this.tb_ContractHash.Text);
        }

        private void Gen(object sender, RoutedEventArgs e)
        {
            BigInteger k = ThinNeo.Helper_BigInt.NextBigInteger(8 * 32);
            var priKey = k.ToByteArray().Take(32).ToArray();
            var wif = ThinNeo.Helper_NEO.GetWifFromPrivateKey(priKey);
            var pubKey = ThinNeo.Helper_NEO.GetPublicKey_FromPrivateKey(priKey);
            var scriptHash = ThinNeo.Helper_NEO.GetScriptHash_FromPublicKey(pubKey);
            var address = ThinNeo.Helper_NEO.GetAddress_FromPublicKey(pubKey);

            this.tb_priKey.Text = ThinNeo.Helper.Bytes2HexString(priKey);
            this.tb_wif.Text = wif;
            this.tb_pubKey.Text = ThinNeo.Helper.Bytes2HexString(pubKey);
            this.tb_address.Text = address;
            this.tb_scriptHash.Text = scriptHash.ToString();
        }

        private new void Name(object sender, RoutedEventArgs e)
        {
            byte[] script = acc_ScriptPackage.GetScript_Name();
            JObject result = wallet.nelApiHelper.InvokeScript(Helper.Bytes2HexString(script));
            string name =System.Text.UTF8Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes(result["result"][0]["stack"][0]["value"].ToString()));
            this.lb_name.Content = name;
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            string id = this.tb_id.Text;
            string pubKey = this.tb_pubKey_set.Text;

            byte[] script = acc_ScriptPackage.GetScript_Create(id,pubKey);
            MakeTran(script);
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            string id = this.tb_id.Text;
            string pubKey = this.tb_pubKey_set.Text;

            byte[] script = acc_ScriptPackage.GetScript_Updatekey(id, pubKey);
            MakeTran(script);
        }

        private void GetKey(object sender, RoutedEventArgs e)
        {
            string id = this.tb_id.Text;

            byte[] script = acc_ScriptPackage.GetScript_Getkey(id);

            Invoke(script);
        }

        private void GetScriptHash(object sender, RoutedEventArgs e)
        {
            string id = this.tb_id.Text;

            byte[] script = acc_ScriptPackage.GetScript_Getkey_scripthash(id);

            Invoke(script);
        }

        private void GetAddressFromId(object sender, RoutedEventArgs e)
        {
            string id = this.tb_id_id2address.Text;

            byte[] script = CreateScriptById(id);

            RIPEMD160Managed ripemd160 = new RIPEMD160Managed();
            var scripthash = Helper.Sha256.ComputeHash(script);
            scripthash = ripemd160.ComputeHash(scripthash);
            var addr = ThinNeo.Helper_NEO.GetAddress_FromScriptHash(scripthash);
            this.tb_address_id2address.Text = addr;
        }
        private byte[] CreateScriptById(string id)
        {
            JArray inputJA = JArray.Parse(string.Format(@"
                    [
	                    '(str)getkey',
	                    [
		                    '(str){0}'
	                    ]
                    ]", id));
            byte[] script = null;//生成特殊账户
            {
                ScriptBuilder sb = new ScriptBuilder();
                for (int i = inputJA.Count - 1; i >= 0; i--)
                {
                    sb.EmitParamJson(inputJA[i]);
                }
                sb.EmitAppCall(new Hash160(this.tb_ContractHash.Text));
                sb.Emit(ThinNeo.VM.OpCode.CHECKSIG);
                script = sb.ToArray();
            }
            return script;
        }

        private void Transfer(object sender, RoutedEventArgs e)
        {
            string from = this.tb_from.Text;
            string to = this.tb_to.Text;
            decimal value = decimal.Parse(this.tb_value.Text);

            var tran = wallet.MakeContractTransactionNoSign(from,to,value);

            var signdata = ThinNeo.Helper_NEO.Sign(tran.GetMessage(), wallet.PriKey);

            var sb = new ThinNeo.ScriptBuilder();
            sb.EmitPushBytes(signdata);
            var iscript = sb.ToArray();

            string id = this.tb_id_id2address.Text;
            var vscript = CreateScriptById(id);

            tran.AddWitnessScript(vscript, iscript);

            Console.WriteLine(Helper.Bytes2HexString(tran.GetRawData()));
            JObject result = wallet.nelApiHelper.SendRawTransaction(Helper.Bytes2HexString(tran.GetRawData()));
            //根据结果做出不同的操作
            AnalysisResult(result);
        }
    }
}
