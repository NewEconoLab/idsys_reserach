using System;

namespace eventserver
{
    class Program
    {
        //accmgr ver 0x89d71f0d96ae9538d56090d1660835d8ea5457d1

        //id server 是一個server，我可以在這上面申請一個ID
        //這個ID 有一個RootKey，可以被改變
        //這個ID 有一個RootID，不會改變，ID確定，就確定

        //id<-->rootid 一對一

        //根據 RootKey/RootPubkey
        //產生多個不同鏈的key/pubkey
        //key_neo/pubkey_neo
        //key_eth/pubkey_eth
        //根據RootID
        //產生多個不同區塊鏈上的地址
        //address_neo
        //address_eth
        //每個鏈上存儲一份map《rooid,pubkey_thischain>
        //login 
        //去指定練上執行合約，login rootid，會產生一條login信息需要簽名
        //用pubkey_thischain 對應的簽名就可以，簽名可以切換成其他方式
        //轉賬
        //從這個地址中轉帳，需要key_neo的簽名
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
