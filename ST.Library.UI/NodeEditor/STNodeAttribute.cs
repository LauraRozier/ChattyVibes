using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ST.Library.UI.NodeEditor
{
    /// <summary>
    /// STNode node characteristics
    /// Used to describe STNode developer information and some behaviors
    /// </summary>
    public class STNodeAttribute : Attribute
    {
        private string _Path;
        /// <summary>
        /// Get the expected path of the STNode node in the tree control
        /// </summary>
        public string Path {
            get { return _Path; }
        }

        private string _Author;
        /// <summary>
        /// Get the author name of the STNode node
        /// </summary>
        public string Author {
            get { return _Author; }
        }

        private string _Mail;
        /// <summary>
        /// Get the author's email address of the STNode node
        /// </summary>
        public string Mail {
            get { return _Mail; }
        }

        private string _Link;
        /// <summary>
        /// Get the author link of the STNode node
        /// </summary>
        public string Link {
            get { return _Link; }
        }

        private string _Description;
        /// <summary>
        /// Get the description information of the STNode node
        /// </summary>
        public string Description {
            get { return _Description; }
        }

        private static char[] m_ch_splitter = new char[] { '/', '\\' };
        private static Regex m_reg = new Regex(@"^https?://", RegexOptions.IgnoreCase);

        /// <summary>
        /// Construct a STNode characteristic
        /// </summary>
        /// <param name="strPath">Expected path</param>
        public STNodeAttribute(string strPath) : this(strPath, null, null, null, null) { }

        /// <summary>
        /// Construct a STNode characteristic
        /// </summary>
        /// <param name="strPath">Expected path</param>
        /// <param name="strDescription">Description</param>
        public STNodeAttribute(string strPath, string strDescription) : this(strPath, null, null, null, strDescription) { }

        /// <summary>
        /// 构造一个STNode特性
        /// </summary>
        /// <param name="strPath">Expected path</param>
        /// <param name="strAuthor">STNode author name</param>
        /// <param name="strMail">STNode Author Email</param>
        /// <param name="strLink">STNode author link</param>
        /// <param name="strDescription">STNode node description information</param>
        public STNodeAttribute(string strPath, string strAuthor, string strMail, string strLink, string strDescription) {
            if (!string.IsNullOrEmpty(strPath))
                strPath = strPath.Trim().Trim(m_ch_splitter).Trim();

            _Path = strPath;
            _Author = strAuthor;
            _Mail = strMail;
            _Description = strDescription;

            if (string.IsNullOrEmpty(strLink) || strLink.Trim() == string.Empty)
                return;

            strLink = strLink.Trim();
            _Link = m_reg.IsMatch(strLink) ? strLink : "http://" + strLink;
        }

        private static Dictionary<Type, MethodInfo> m_dic = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Helper function to get the type
        /// </summary>
        /// <param name="stNodeType">Node type</param>
        /// <returns>Function information</returns>
        public static MethodInfo GetHelpMethod(Type stNodeType) {
            if (m_dic.ContainsKey(stNodeType))
                return m_dic[stNodeType];

            var mi = stNodeType.GetMethod("ShowHelpInfo");

            if (mi == null)
                return null;

            if (!mi.IsStatic)
                return null;

            var ps = mi.GetParameters();

            if (ps.Length != 1)
                return null;

            if (ps[0].ParameterType != typeof(string))
                return null;

            m_dic.Add(stNodeType, mi);
            return mi;
        }

        /// <summary>
        /// Execute the helper function corresponding to the node type
        /// </summary>
        /// <param name="stNodeType">Node type</param>
        public static void ShowHelp(Type stNodeType) {
            var mi = STNodeAttribute.GetHelpMethod(stNodeType);

            if (mi == null)
                return;

            mi.Invoke(null, new object[] { stNodeType.Module.FullyQualifiedName });
        }
    }
}
