using RaceDay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaceDay.ViewModels
{
	public class BaseViewModel
	{
		public PageMessageModel PageMessage { get; set; }
		public MFUser User
		{
			get
			{
				return HttpContext.Current.User as MFUser;
			}
		}
	}
}