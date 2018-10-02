using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace QUALITY.MyForms
{
    public partial class frmThongTinCheckListChiTiet : DevExpress.XtraEditors.XtraForm
    {

        #region values_private
        long _IDCheckListMau = -1;
        long _DrawFileID = -1;
        long _IDCheckList = -1;
        long _IDNoiDungKiem = -1;
        int _LanKiem = -1;


        Timer _timer1;
        #endregion

        public frmThongTinCheckListChiTiet(long IDCheckList, int LanKiem, long IDCheckListMau, long DrawFileID)
        {
            InitializeComponent();
            _IDCheckListMau = IDCheckListMau;
            _DrawFileID = DrawFileID;
            _IDCheckList = IDCheckList;
            _LanKiem = LanKiem;
            // Hình ảnh mặc định
            imageEdit1.Image = imageCollection1.Images["edit_32x32.png"];
        }

        public delegate void dlOKCheckListDetails();
        public dlOKCheckListDetails OKCheckListDetails;
        public void loadData()
        {
            CopyParseValue();
            // load focus table
            gvCheckList.FocusedRowHandle = 0;
            gvCheckList.FocusedColumn = gvCheckList.Columns["MaLoi"];
            gvCheckList.ShowEditor();
        }
        public byte[] _FileValue = null;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)

        {

            if (keyData == Keys.F5)
            {

                using (frmHienThiLoi frm = new frmHienThiLoi((long)_IDNoiDungKiem))
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (gvCheckList.FocusedColumn.FieldName == "MaLoi")
                        {
                            gvCheckList.SetFocusedRowCellValue("MaLoi", frm._MaLoi);

                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }

        void ShowDofView()
        {
            lIBRARY_FILE_DRAWINGTableAdapter.FillByDrawFileID(dsCheckListMau.LIBRARY_FILE_DRAWING, (int)_DrawFileID);

            MemoryStream memory = new MemoryStream(_FileValue);
            memory.Write(_FileValue, 0, _FileValue.Length);

            vBanVe.LoadDocument(memory);
        }
        #region method
        void CopyParseValue()
        {
            // dsCheckList.QUALITY_CHECKLIST_CHITIET.Clear();
            // GET SỐ row của check_list_mẫu
            // TODO: This line of code loads data into the 'dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET' table. You can move, or remove it, as needed.
            this.qUALITY_CHECKLIST_MAU_CHITIETTableAdapter.FillByIDCheckListMau(this.dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET, _IDCheckListMau);
            int row_count = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Count;
            for (int i = 0; i < row_count; i++)
            {
                DataRow newRow = dsCheckList.QUALITY_CHECKLIST_CHITIET.NewRow();
                newRow["TenNoiDung"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["TenNoiDung"];
                newRow["YeuCau"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["YeuCau"];
                newRow["ThuTu"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["ThuTu"];
                newRow["DungSaiMax"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["DungSaiMax"];
                newRow["DungSaiMin"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["DungSaiMin"];
                newRow["IDNoiDungKiem"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["IDNoiDungKiem"];
                newRow["IDCheckList"] = _IDCheckList;
                newRow["MaThietBi"] = dsCheckListMau.QUALITY_CHECKLIST_MAU_CHITIET.Rows[i]["MaThietBi"];
                newRow["LanKiem"] = _LanKiem;
                newRow["Dat"] = 1;
                dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows.Add(newRow);
            }
        }

        void LoadShowValueLoaiDungSai_NoiDungKiem_Loi()
        {
            try
            {
                _IDNoiDungKiem = Convert.ToInt64(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "IDNoiDungKiem"));

                txtNoiDungKiem.Text = Convert.ToString(qUALITY_CHECKLIST_CHITIETTableAdapter.QUALITY_GET_TENLOAIDUNGSAI_BY_IDNOIDUNG(_IDNoiDungKiem));
                txtLoaiDungSai.Text = Convert.ToString(qUALITY_CHECKLIST_CHITIETTableAdapter.QUALITY_GET_TENNOIDUNG_BY_IDNOIDUNG(_IDNoiDungKiem));
                string _MaLoi = Convert.ToString(dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["MaLoi"]);
                txtLoi.Text = Convert.ToString(qUALITY_CHECKLIST_CHITIETTableAdapter.GET_TenLoi_BY_MaLoi(_MaLoi));

            }
            catch (Exception ex)
            {
            }
           
        }

        void CheckValueInput_ViewColor()
        {
            foreach (   QUALITY.MyDatasets.dsCheckList.QUALITY_CHECKLIST_CHITIETRow
            item in dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows)
            {
                if (!item.Dat)
                {
                    imageEdit1.Image = imageCollection1.Images["bugreport_32x32.png"];
                    return;
                }
            }
            imageEdit1.Image = imageCollection1.Images["apply_32x32.png"];
        }
        void ShowViewData()
        {
            string _MaLoi = Convert.ToString(dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["MaLoi"]);
            string _ThuTe = Convert.ToString(dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["ThucTe"]);
            if (_MaLoi != "" || _ThuTe != "")
            {
                double _yeucau = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "YeuCau"));
                double _min = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "DungSaiMin"));
                double _max = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "DungSaiMax"));
                double _thucte = toDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "ThucTe"));

                if ((_yeucau + _min) <= _thucte && (_yeucau + _max) >= _thucte)
                {
                    imageEdit1.Image = imageCollection1.Images["apply_32x32.png"];
                    gvCheckList.SetRowCellValue(gvCheckList.FocusedRowHandle, "Dat", true);
                }
                else // lỗi
                {
                    imageEdit1.Image = imageCollection1.Images["bugreport_32x32.png"];
                    gvCheckList.SetRowCellValue(gvCheckList.FocusedRowHandle, "Dat", false);
                }
                //gvCheckList_RowStyle(null, null);
            }
            else if(_MaLoi == "" && _ThuTe =="")
            {
                // nếu không có hiện tượng gì xét trạng thái [Dạt] = true   ;
                dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["Dat"] = true;
            }
        }
        #endregion

        private void frmThongTinCheckListChiTiet_Load(object sender, EventArgs e)
        {

            try
            {
                _timer1 = new Timer();
                _timer1.Tick += Timer1_Tick;
                _timer1.Start();

                Int64? checkexist = Convert.ToInt64(qUALITY_CHECKLIST_CHITIETTableAdapter.GET_IsExistIDCheckListChiTiet_BY_IDCheckList_LanKiem(_IDCheckList, _LanKiem));
                if (checkexist > 0)
                {
                    qUALITY_CHECKLIST_CHITIETTableAdapter.FillByIDCheckList_LanKiem(dsCheckList.QUALITY_CHECKLIST_CHITIET, _IDCheckList, _LanKiem);
                }
                else// chưa tồn tại
                {
                    CopyParseValue();
                   
                }
                // hiển thị giá trị
                LoadShowValueLoaiDungSai_NoiDungKiem_Loi();
                CheckValueInput_ViewColor()
;                // load focus table
                gvCheckList.FocusedRowHandle = 0;
                gvCheckList.FocusedColumn = gvCheckList.Columns["MaLoi"];
                gvCheckList.Focus();
              

            }
            catch (Exception ex)
            {
                // throw;
            }
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            ShowDofView();
            _timer1.Stop();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                qUALITYCHECKLISTCHITIETBindingSource.EndEdit();
                qUALITY_CHECKLIST_CHITIETTableAdapter.Update(dsCheckList.QUALITY_CHECKLIST_CHITIET);
                dsCheckList.QUALITY_CHECKLIST_CHITIET.AcceptChanges();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }



        private void btnThemMoiDungKiem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmHienThiBanVe frm = new frmHienThiBanVe();
            frm._FileValue = _FileValue;
            frm.Show();
        }


        private void repDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }



        private void glkeLoaiDS_EditValueChanged(object sender, EventArgs e)
        {


        }
        double toDouble(object o)
        {
            try
            {
                return Convert.ToDouble(o);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        bool isNull(object o)
        {
            return (o == null || o is DBNull);
        }

        private void grTTBV_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void frmThongTinCheckListChiTiet_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //this.Hide();
        }

        private void gvCheckList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string _MaLoi = Convert.ToString(dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["MaLoi"]);
            string _ThuTe = Convert.ToString(dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["ThucTe"]);
            if (e.Column.FieldName == "MaLoi")
            {
                if (_ThuTe == "" && _MaLoi=="")
                {
                    dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["Dat"] = true;
                    imageEdit1.Image = imageCollection1.Images["apply_32x32.png"];
                }
                else
                {
                dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["Dat"] = false;
                    imageEdit1.Image = imageCollection1.Images["bugreport_32x32.png"];
                }
            }
            if (e.Column.FieldName == "ThucTe")
            {
                if (_MaLoi == "" && _ThuTe == "")
                {
                    // nếu không có hiện tượng gì xét trạng thái [Dạt] = true   ;
                    dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["Dat"] = true;
                }
               else /*(_MaLoi != "" || _ThuTe != "")*/
                {
                    double _yeucau = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "YeuCau"));
                    double _min = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "DungSaiMin"));
                    double _max = Convert.ToDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "DungSaiMax"));
                    double _thucte = toDouble(gvCheckList.GetRowCellValue(gvCheckList.FocusedRowHandle, "ThucTe"));

                    if ((_yeucau + _min) <= _thucte && (_yeucau + _max) >= _thucte)
                    {
                        imageEdit1.Image = imageCollection1.Images["apply_32x32.png"];
                        gvCheckList.SetRowCellValue(gvCheckList.FocusedRowHandle, "Dat", true);
                    }
                    else // lỗi
                    {
                        imageEdit1.Image = imageCollection1.Images["bugreport_32x32.png"];
                        gvCheckList.SetRowCellValue(gvCheckList.FocusedRowHandle, "Dat", false);
                    }
                   
                }
              

            }
            btnLuu_Click(null, null);
        }


        //private void gvCheckList_ShownEditor(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    if (gvCheckList.FocusedColumn.FieldName =="MaLoi")
        //    //    {
        //    //        gvCheckList_RowClick(null, null);
        //    //    }
        //    //}
        //    //catch (Exception)
        //    //{

        //    //    //throw;
        //    //}
        //}

        private void gcCheckList_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    (gcCheckList.FocusedView as ColumnView).FocusedRowHandle++;
                    e.Handled = true;
                }
                if (e.KeyData == Keys.Escape && gvCheckList.FocusedColumn.FieldName =="MaLoi")
                {
                    dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["MaLoi"] = "";
                    dsCheckList.QUALITY_CHECKLIST_CHITIET.Rows[gvCheckList.FocusedRowHandle]["ThucTe"] = DBNull.Value;
                    gvCheckList.SetRowCellValue(gvCheckList.FocusedRowHandle, "Dat", true);
                    btnLuu_Click(null, null);
                    CheckValueInput_ViewColor();
                }
            }
            catch (Exception ex)
            {

                
            }
          
        }

        private void gvCheckList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            LoadShowValueLoaiDungSai_NoiDungKiem_Loi();
        }

        private void gvCheckList_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ThucTe" || e.Column.FieldName == "MaLoi")
                    {
                        if (Convert.ToBoolean(View.GetRowCellValue(e.RowHandle, View.Columns["Dat"])))
                        {

                            e.Appearance.BackColor = Color.FromArgb(102, 187, 106);
                        }
                        else // lỗi
                        {
                            e.Appearance.BackColor = Color.FromArgb(239, 83, 80);
                        }
                    }
                    
                }
            }
            catch (Exception)
            {

                // throw;
            }
        }
    }
}