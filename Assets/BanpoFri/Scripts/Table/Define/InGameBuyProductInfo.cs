using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class InGameBuyProductInfoData
    {
        [SerializeField]
		private int _product_idx;
		public int product_idx
		{
			get { return _product_idx;}
			set { _product_idx = value;}
		}
		[SerializeField]
		private int _price;
		public int price
		{
			get { return _price;}
			set { _price = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}

    }

    [System.Serializable]
    public class InGameBuyProductInfo : Table<InGameBuyProductInfoData, int>
    {
    }
}

