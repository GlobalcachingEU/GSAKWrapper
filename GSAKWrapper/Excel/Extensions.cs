using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public static class Extensions
    {
        public static IRow GetOrCreateRow(this ISheet sheet, int row)
        {
            IRow result = sheet.GetRow(row);
            if (result == null)
            {
                result = sheet.CreateRow(row);
            }
            return result;
        }
        public static ICell GetOrCreateCol(this IRow row, int col)
        {
            ICell result = row.GetCell(col);
            if (result == null)
            {
                result = row.CreateCell(col);
            }
            return result;
        }
        public static ICell GetOrCreateCell(this ISheet sheet, int row, int col)
        {
            return sheet.GetOrCreateRow(row).GetOrCreateCol(col);
        }
    }
}
