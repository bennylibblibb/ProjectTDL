using System.Configuration;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web.UI;

namespace JC_SoccerWeb.Common
{
	public class ResourceHelper
	{
		private ResourceManager _rm = null;
		private Page _page = null;

		public ResourceHelper(Page page)
		{
			_page = page;
			PrepareResource();
		}

		/**/

		/// <summary>
		/// 获得当前文化，从Session["HWcultureName"]取
		/// </summary>
		protected CultureInfo GetCurrentCulture
		{
			get
			{
				if (_page.Session["HWCultureName"] != null)
					return CultureInfo.CreateSpecificCulture(_page.Session["HWCultureName"].ToString());
				else //如果Session未设置，则直接从web.config中读取默认设置
					return CultureInfo.CreateSpecificCulture(ConfigurationSettings.AppSettings["CultureName"]);
			}
		}

		/**/

		/// <summary>
		/// 初始化资源管理
		/// </summary>
		public void PrepareResource()
		{
			_rm =
				ResourceManager.CreateFileBasedResourceManager("strings",
				                                               _page.Server.MapPath("resources") + Path.DirectorySeparatorChar, null);
		}

		/**/

		/// <summary>
		/// 获得资源指针
		/// </summary>
		protected ResourceManager resource
		{
			get
			{
				if (_rm == null)
					PrepareResource();
				return _rm;
			}
		}

		/**/

		/// <summary>
		/// 获得资源字符串
		/// </summary>
		/// <param name="ResourceID">资源ID</param>
		/// <returns></returns>
		public string GetString(string ResourceID)
		{
			return resource.GetString(ResourceID, GetCurrentCulture);
		}
	}
}