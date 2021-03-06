using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminPermissionAdd : BasePageCms
    {
        public CheckBoxList CblWebsitePermissions;
        public CheckBoxList CblChannelPermissions;
        public Literal LtlNodeTree;

        public PlaceHolder PhWebsitePermissions;
        public PlaceHolder PhChannelPermissions;

        public static string GetRedirectUrl(int siteId, string roleName)
        {
            var queryString = new NameValueCollection { { "SiteId", siteId.ToString() } };
            if (!string.IsNullOrEmpty(roleName))
            {
                queryString.Add("RoleName", roleName);
            }

            return PageUtils.GetSettingsUrl(nameof(PageAdminPermissionAdd), queryString);
        }

        private string GetNodeTreeHtml()
        {
            var htmlBuilder = new StringBuilder();
            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList == null)
            {
                PageUtils.RedirectToErrorPage("超出时间范围，请重新进入！");
                return string.Empty;
            }
            var channelIdList = new List<int>();
            foreach (var systemPermissionsInfo in systemPermissionsInfoList)
            {
                channelIdList.AddRange(TranslateUtils.StringCollectionToIntList(systemPermissionsInfo.ChannelIdCollection));
            }

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            htmlBuilder.Append("<span id='ChannelSelectControl'>");
            var theChannelIdList = DataProvider.ChannelDao.GetIdListBySiteId(SiteId);
            var isLastNodeArray = new bool[theChannelIdList.Count];
            foreach (var theChannelId in theChannelIdList)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, theChannelId);
                htmlBuilder.Append(GetTitle(nodeInfo, treeDirectoryUrl, isLastNodeArray, channelIdList));
                htmlBuilder.Append("<br/>");
            }
            htmlBuilder.Append("</span>");
            return htmlBuilder.ToString();
        }

        private string GetTitle(ChannelInfo nodeInfo, string treeDirectoryUrl, IList<bool> isLastNodeArray, ICollection<int> channelIdList)
        {
            var itemBuilder = new StringBuilder();
            if (nodeInfo.Id == SiteId)
            {
                nodeInfo.IsLastNode = true;
            }
            if (nodeInfo.IsLastNode == false)
            {
                isLastNodeArray[nodeInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[nodeInfo.ParentsCount] = true;
            }
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            if (nodeInfo.IsLastNode)
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            else
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }

            var check = "";
            if (channelIdList.Contains(nodeInfo.Id))
            {
                check = "checked";
            }

            var disabled = "";
            if (!IsOwningChannelId(nodeInfo.Id))
            {
                disabled = "disabled";
                check = "";
            }

            itemBuilder.Append($@"
<span class=""checkbox checkbox-primary"" style=""padding-left: 0px;"">
    <input type=""checkbox"" id=""ChannelIdCollection_{nodeInfo.Id}"" name=""ChannelIdCollection"" value=""{nodeInfo.Id}"" {check} {disabled}/>
    <label for=""ChannelIdCollection_{nodeInfo.Id}""> {nodeInfo.ChannelName} &nbsp;<span style=""font-size:8pt;font-family:arial"" class=""gray"">({nodeInfo.ContentNum})</span></label>
</span>
");

            return itemBuilder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissioins = PermissionsManager.GetPermissions(Body.AdminName);
            LtlNodeTree.Text = GetNodeTreeHtml();

            if (IsPostBack) return;

            PermissionsManager.VerifyAdministratorPermissions(Body.AdminName, ConfigManager.Permissions.Settings.Admin);

            if (permissioins.IsSystemAdministrator)
            {
                var channelPermissions = PermissionConfigManager.Instance.ChannelPermissions;
                foreach (var permission in channelPermissions)
                {
                    if (permission.Name == ConfigManager.Permissions.Channel.ContentCheckLevel1)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 1)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.Permissions.Channel.ContentCheckLevel2)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 2)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.Permissions.Channel.ContentCheckLevel3)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 3)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.Permissions.Channel.ContentCheckLevel4)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 4)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.Permissions.Channel.ContentCheckLevel5)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 5)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    var listItem = new ListItem(permission.Text, permission.Name);
                    CblChannelPermissions.Items.Add(listItem);
                }
            }
            else
            {
                PhChannelPermissions.Visible = false;
                if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(SiteId))
                {
                    var channelPermissions = ProductPermissionsManager.Current.ChannelPermissionDict[SiteId];
                    foreach (var channelPermission in channelPermissions)
                    {
                        foreach (var permission in PermissionConfigManager.Instance.ChannelPermissions)
                        {
                            if (permission.Name == channelPermission)
                            {
                                if (channelPermission == ConfigManager.Permissions.Channel.ContentCheck)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel) continue;
                                }
                                else if (channelPermission == ConfigManager.Permissions.Channel.ContentCheckLevel1)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 1) continue;
                                }
                                else if (channelPermission == ConfigManager.Permissions.Channel.ContentCheckLevel2)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 2) continue;
                                }
                                else if (channelPermission == ConfigManager.Permissions.Channel.ContentCheckLevel3)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 3) continue;
                                }
                                else if (channelPermission == ConfigManager.Permissions.Channel.ContentCheckLevel4)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 4) continue;
                                }
                                else if (channelPermission == ConfigManager.Permissions.Channel.ContentCheckLevel5)
                                {
                                    if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 5) continue;
                                }

                                PhChannelPermissions.Visible = true;
                                var listItem = new ListItem(permission.Text, permission.Name);
                                CblChannelPermissions.Items.Add(listItem);
                            }
                        }
                    }
                }
            }

            if (permissioins.IsSystemAdministrator)
            {
                var websitePermissions = PermissionConfigManager.Instance.WebsitePermissions;
                foreach (var permission in websitePermissions)
                {
                    var listItem = new ListItem(permission.Text, permission.Name);
                    CblWebsitePermissions.Items.Add(listItem);
                }
            }
            else
            {
                PhWebsitePermissions.Visible = false;
                if (ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(SiteId))
                {
                    var websitePermissionList = ProductPermissionsManager.Current.WebsitePermissionDict[SiteId];
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in PermissionConfigManager.Instance.WebsitePermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                PhWebsitePermissions.Visible = true;
                                var listItem = new ListItem(permission.Text, permission.Name);
                                CblWebsitePermissions.Items.Add(listItem);
                            }
                        }
                    }
                }
            }

            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList != null)
            {
                SitePermissionsInfo systemPermissionsInfo = null;
                foreach (var sitePermissionsInfo in systemPermissionsInfoList)
                {
                    if (sitePermissionsInfo.SiteId == SiteId)
                    {
                        systemPermissionsInfo = sitePermissionsInfo;
                        break;
                    }
                }
                if (systemPermissionsInfo == null) return;

                foreach (ListItem item in CblChannelPermissions.Items)
                {
                    item.Selected = CompareUtils.Contains(systemPermissionsInfo.ChannelPermissions, item.Value);
                }
                foreach (ListItem item in CblWebsitePermissions.Items)
                {
                    item.Selected = CompareUtils.Contains(systemPermissionsInfo.WebsitePermissions, item.Value);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList != null)
            {
                var systemPermissionlist = new List<SitePermissionsInfo>();
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    if (systemPermissionsInfo.SiteId == SiteId)
                    {
                        continue;
                    }
                    systemPermissionlist.Add(systemPermissionsInfo);
                }

                var channelIdList = TranslateUtils.StringCollectionToStringList(Request.Form["ChannelIdCollection"]);
                if (channelIdList.Count > 0 && CblChannelPermissions.SelectedItem != null || CblWebsitePermissions.SelectedItem != null)
                {
                    var systemPermissionsInfo = new SitePermissionsInfo
                    {
                        SiteId = SiteId,
                        ChannelIdCollection = TranslateUtils.ObjectCollectionToString(channelIdList),
                        ChannelPermissions =
                            ControlUtils.SelectedItemsValueToStringCollection(CblChannelPermissions.Items),
                        WebsitePermissions =
                            ControlUtils.SelectedItemsValueToStringCollection(CblWebsitePermissions.Items)
                    };

                    systemPermissionlist.Add(systemPermissionsInfo);
                }

                Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] = systemPermissionlist;
            }

            PageUtils.Redirect(PageAdminRoleAdd.GetReturnRedirectUrl(Body.GetQueryString("RoleName")));
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageAdminRoleAdd.GetReturnRedirectUrl(Body.GetQueryString("RoleName")));
        }
    }
}
