using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using SortDirection = System.Web.UI.WebControls.SortDirection;

namespace Net.Web
{
    public static class WebGrid
    {
        public static WebGrid<T> Bind<T>(IEnumerable<T> list)
        {
            return new WebGrid<T>().Bind(list.AsQueryable());
        }
    }
    /// <summary>
    /// Wrapper for System.Web.RenderSectionExtension.WebGrid that preserves the item type from the data source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebGrid<T> : System.Web.Helpers.WebGrid
    {
        private readonly SortDirection _defaultSortDirection = SortDirection.Ascending;

        private SortDirection? _sortDirection;

        /// <param name="columnNames">Data source column names. Auto-populated by default.</param>
        /// <param name="defaultSort">Default sort column.</param>
        /// <param name="defaultSortDirection"> </param>
        /// <param name="rowsPerPage">Number of rows per page.</param>
        /// <param name="canPage">true to enable paging</param>
        /// <param name="canSort">true to enable sorting</param>
        /// <param name="ajaxUpdateContainerId">ID for the grid's container element. This enables AJAX support.</param>
        /// <param name="ajaxUpdateCallback">Callback function for the AJAX functionality once the update is complete</param>
        /// <param name="fieldNamePrefix">Prefix for query string fields to support multiple grids.</param>
        /// <param name="pageFieldName">Query string field name for page number.</param>
        /// <param name="selectionFieldName">Query string field name for selected row number.</param>
        /// <param name="sortFieldName">Query string field name for sort column.</param>
        /// <param name="sortDirectionFieldName">Query string field name for sort direction.</param>
        public WebGrid(IEnumerable<string> columnNames = null, string defaultSort = null, SortDirection defaultSortDirection = SortDirection.Ascending, int rowsPerPage = 10, bool canPage = true, bool canSort = true, string ajaxUpdateContainerId = null, string ajaxUpdateCallback = null, string fieldNamePrefix = null, string pageFieldName = null, string selectionFieldName = null, string sortFieldName = null, string sortDirectionFieldName = null)
            : base(null, columnNames, defaultSort, rowsPerPage, canPage, canSort, ajaxUpdateContainerId, ajaxUpdateCallback, fieldNamePrefix, pageFieldName, selectionFieldName, sortFieldName, sortDirectionFieldName)
        {
            _defaultSortDirection = defaultSortDirection;
        }

        public WebGridColumn Col(string columnName = null, string header = null, Func<T, object> format = null, string style = null, bool canSort = true)
        {
            Func<dynamic, object> wrappedFormat = null;
            if (format != null)
            {
                wrappedFormat = o => format((T)o.Value);
            }
            WebGridColumn column = Column(columnName, header, wrappedFormat, style, canSort);
            return column;
        }

        public WebGrid<T> Bind(IQueryable<T> source, IEnumerable<string> columnNames = null, bool autoSortAndPage = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (autoSortAndPage)
            {
                Bind(source.Cast<dynamic>(), columnNames, true, -1);
                return this;
            }

            var rowCount = source.Count();

            if (!string.IsNullOrWhiteSpace(SortColumn))
            {
                // TODO add nuget pacakage DynamicQuery
                //source = source.OrderBy(SortColumn + (SortDirection == SortDirection.Ascending ? " asc" : " desc"));
            }

            SortDirection = SortDirection;

            source = source.Skip(PageIndex * RowsPerPage).Take(RowsPerPage);

            Bind(source.ToList().Cast<dynamic>(), columnNames, false, rowCount);
            return this;
        }

        private static HttpContextBase HttpContext
        {
            get { return new HttpContextWrapper(System.Web.HttpContext.Current); }
        }

        private static NameValueCollection QueryString
        {
            get { return HttpContext.Request.QueryString; }
        }

        public new SortDirection SortDirection
        {
            get
            {
                if (_sortDirection != null)
                    return _sortDirection.Value;

                string sortDirection = QueryString[SortDirectionFieldName];

                if (sortDirection != null)
                {
                    if (sortDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase) ||
                        sortDirection.Equals("ASCENDING", StringComparison.OrdinalIgnoreCase))
                    {
                        _sortDirection = SortDirection.Ascending;
                    }

                    if (sortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase) ||
                        sortDirection.Equals("DESCENDING", StringComparison.OrdinalIgnoreCase))
                    {
                        _sortDirection = SortDirection.Descending;
                    }
                }

                return _sortDirection ?? _defaultSortDirection;
            }
            set
            {
                _sortDirection = value;
            }
        }
    }
}
