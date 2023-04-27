using BarcodeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection oSqlConnection;
        SqlCommand oSqlCommand;
        SqlDataAdapter oSqlDataAdapter;
        public string sqlQuery;
        public string UserValue = "";
        public string cons = ConfigurationManager.AppSettings["cons"];
        public string loc = ConfigurationManager.AppSettings["loc"];
        private void LoadPersonal()
        {
            DateTime batchdate = new DateTime();
            DataSet odsLoadLoanAppliedPersonal = new DataSet();
            odsLoadLoanAppliedPersonal.Clear();
            dataGridView1.Rows.Clear();
            DataTable oDataSetv6 = new DataTable();
            oSqlConnection = new SqlConnection(cons);
            oSqlCommand = new SqlCommand();
            sqlQuery = " SELECT max(c.initials)inililes,e.TubeCategory,a.TestSID,e.CategoryID,a.pdid,e.CategoryName,max(a.Issued) Issued,CAST(a.[RequestedTime]  as DATE) " +
 " as RequestedTime,  max(c.surname)  sname ,max(b.RNK_NAME) rnkname, max(c.ServiceNo) sno  , max(c.RelationshipType)" +
 "  relasiont, max(h.Relationship)  relasiondet FROM[MMS].[dbo] .[Lab_Report] as a with(nolock)  inner join[MMS].[dbo].[Lab_SubCategory] as d" +
" on d.[LabTestID]=a.[LabTestID] inner join[MMS].[dbo].  [Lab_MainCategory] as e on e.CategoryID=d.CategoryID inner join[MMS].[dbo]." +
"   [Patient_Detail] as l on l.pdid=a.PDID inner join[MMS].[dbo].[Patient] as c on l.pid=c.pid left join[MMS].[dbo].[RelationshipType] as h on h.RTypeID=c.RelationshipType" +
" left join[MMS].[dbo].[ranks] as b on b.RANK=c.RANK " +
" where a.RequestedLocID='"+ loc + "' and a.IsPrint='0' and convert(date, a.RequestedTime)=convert(varchar,'" + dateTimePicker1.Value.Date.ToString() + "',111) group by " +
" e.CategoryName, a.TestSID, CAST(a.[RequestedTime] as DATE),e.CategoryID,e.TubeCategory,a.pdid order by e.TubeCategory, a.TestSID desc";
            // sqlQuery = " SELECT TOP 1 locdata FROM vtsdata where devid='" + devid + "' and  createddate between   '" + dt1 + "' and '" + dt2 + "'  ORDER BY vtsid DESC ";
            oSqlCommand.Connection = oSqlConnection;
            oSqlCommand.CommandText = sqlQuery;
            oSqlCommand.CommandTimeout = 120000;
            //   oSqlConnection.Open();
            oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            oSqlDataAdapter.Fill(oDataSetv6);
            // oSqlConnection.Close();
            var lablist = oDataSetv6.AsEnumerable()
    .Select(dataRow => new getlabdata
    {

        tsid = dataRow.Field<string>("TestSID"),
        catid = dataRow.Field<string>("CategoryID"),
        pdids = dataRow.Field<string>("pdid"),
        catname = dataRow.Field<string>("CategoryName"),
        rtime = dataRow.Field<DateTime?>("RequestedTime").ToString(),
        rtimed = dataRow.Field<DateTime?>("RequestedTime"),
        tubecat = dataRow.Field<int?>("TubeCategory"),
        sname = dataRow.Field<string>("sname"),
        rnkname = dataRow.Field<string>("rnkname"),
        sno = dataRow.Field<string>("sno"),
        inililes = dataRow.Field<string>("inililes"),

        relasiont = dataRow.Field<string>("relasiondet"),
    }).ToList();



            // var lablist = (from f in db.Lab_Report.Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year) select f).GroupBy(s => new { s.Lab_SubCategory.Lab_MainCategory.CategoryName, s.PDID, s.TestSID }).Select(g => g.FirstOrDefault()).OrderBy(g => g.TestSID).Select(s => new getlabdata { pdids = s.PDID, inililes = s.Patient_Detail.Patient.Initials, sname = s.Patient_Detail.Patient.Surname, sno = s.Patient_Detail.Patient.ServiceNo, rnkname = s.Patient_Detail.Patient.rank1.RNK_NAME, catname = s.Lab_SubCategory.Lab_MainCategory.CategoryName, tsid = s.TestSID, catid = s.Lab_SubCategory.Lab_MainCategory.CategoryID, relasiont = s.Patient_Detail.Patient.RelationshipType1.Relationship, rtime = s.RequestedTime.ToString(), rtimed = s.RequestedTime,tubecat=s.Lab_SubCategory.Lab_MainCategory.TubeCategory }).OrderByDescending(p => p.rtimed).ToList();
            //db.Lab_Report.Include(l => l.Patient_Detail).Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year);
            //IEnumerable<Lab_Report> filist = lablist.GroupBy(c => new { c.Lab_SubCategory.Lab_MainCategory.CategoryName, c.Lab_SubCategory.Lab_MainCategory.CategoryID, c.PDID, c.TestSID }).Select(group => group.FirstOrDefault()).OrderByDescending(p => p.RequestedTime);
            var i1 = ""; var i2 = ""; int? i3 = 0;
            List<getlabdata> temp = new List<getlabdata>();
            List<getlabdata> temp2 = new List<getlabdata>();

            string lnm = "";
            int cvb = 0;
            foreach (var item in lablist)
            {

                if (i1.Equals(item.tsid) && i3.Equals(item.tubecat))
                {
                    if (temp.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    else if (temp2.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp2.RemoveAt(temp2.Count - 1);
                    }
                    lnm = lnm + "/" + item.catname;
                    item.catname = lnm;
                    temp2.Add(item);
                }
                else
                {
                    lnm = "";
                    cvb++;
                    temp.Add(item);

                    lnm = lnm + "/" + item.catname;
                }
                i1 = item.tsid;
                i2 = item.sno;
                i3 = item.tubecat;

            }

            var joined3 = temp.Concat(temp2).OrderByDescending(d => d.rtimed);

            int count = 0;
                foreach (var item in joined3)
                {
                    
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[count].Cells["ServiceNo"].Value = item.sno;
                    dataGridView1.Rows[count].Cells["Initials"].Value = item.inililes;
                    dataGridView1.Rows[count].Cells["Surname"].Value = item.sname;
                    dataGridView1.Rows[count].Cells["Test"].Value =item.catname;
                    dataGridView1.Rows[count].Cells["pdid"].Value = item.tsid;
                    dataGridView1.Rows[count].Cells["ptype"].Value = item.relasiont;
                    dataGridView1.Rows[count].Cells["rqdate"].Value = item.rtime;
                count++;
            }
            


        }
        private void LoadPersonal2(string id)
        {
            DateTime batchdate = new DateTime();
            DataSet odsLoadLoanAppliedPersonal = new DataSet();
            odsLoadLoanAppliedPersonal.Clear();
            dataGridView1.Rows.Clear();
            DataTable oDataSetv6 = new DataTable();
            oSqlConnection = new SqlConnection(cons);
            oSqlCommand = new SqlCommand();
            sqlQuery = " SELECT e.TubeCategory,a.TestSID,e.CategoryID,a.pdid,e.CategoryName,max(a.Issued) Issued,CAST(a.[RequestedTime] " +
" as DATE) as RequestedTime, COALESCE(NULLIF(concat(max(case when c.RelationshipType = 1 and b.Surname != '0' " +
" then b.Surname end), max(case when c.RelationshipType = 2 then k.SpouseName end), " +
" max(case when c.RelationshipType = 5 and c.DateOfBirth = f.DOB then f.ChildName end), " +
" max(case when c.RelationshipType = 3 and g.Relationship = 'Father' then g.ParentName end), " +
" max(case when c.RelationshipType = 4 and g.Relationship = 'Mother' then g.ParentName end)), ''), max(c.surname)) " +
" sname ,max(case when c.RelationshipType = 1 then b.Rank end) rnkname, max(c.ServiceNo) sno " +
" ,max(case when c.RelationshipType = 1 then b.Initials end) inililes, max(c.RelationshipType) relasiont, max(h.Relationship) " +
                " relasiondet FROM[MMS].[dbo] .[Lab_Report] as a with(nolock) " +
" inner join[MMS].[dbo].[Lab_SubCategory] as d on d.[LabTestID]=a.[LabTestID] inner join[MMS].[dbo]. " +
" [Lab_MainCategory] as e on e.CategoryID=d.CategoryID " +
" inner join[MMS].[dbo].[Patient_Detail] as l on l.pdid=a.PDID inner join[MMS].[dbo].[Patient] as c on l.pid=c.pid " +
" left join[MMS].[dbo].[PersonalDetails] as b on c.ServiceNo=b.ServiceNo " +
" left join[MMS].[dbo].[SpouseDetails] as k on b.SNo=k.SNo " +
" left join[MMS].[dbo].[Children] as f on b.SNo=f.SNo left join[MMS].[dbo].[parents] as g on b.SNo=g.SNo " +
" left join[MMS].[dbo].[RelationshipType] as h on h.RTypeID=c.RelationshipType " +
" where a.RequestedLocID='" + loc + "' and c.serviceno='"+id+"' and a.IsPrint='0' and convert(date, a.RequestedTime)=convert(varchar,'" + dateTimePicker1.Value.Date.ToString() + "',111) group by " +
" e.CategoryName, a.TestSID, CAST(a.[RequestedTime] as DATE),e.CategoryID,e.TubeCategory,a.pdid order by e.TubeCategory, a.TestSID desc";
            // sqlQuery = " SELECT TOP 1 locdata FROM vtsdata where devid='" + devid + "' and  createddate between   '" + dt1 + "' and '" + dt2 + "'  ORDER BY vtsid DESC ";
            oSqlCommand.Connection = oSqlConnection;
            oSqlCommand.CommandText = sqlQuery;
            oSqlCommand.CommandTimeout = 120000;
            //   oSqlConnection.Open();
            oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            oSqlDataAdapter.Fill(oDataSetv6);
            // oSqlConnection.Close();
            var lablist = oDataSetv6.AsEnumerable()
    .Select(dataRow => new getlabdata
    {

        tsid = dataRow.Field<string>("TestSID"),
        catid = dataRow.Field<string>("CategoryID"),
        pdids = dataRow.Field<string>("pdid"),
        catname = dataRow.Field<string>("CategoryName"),
        rtime = dataRow.Field<DateTime?>("RequestedTime").ToString(),
        rtimed = dataRow.Field<DateTime?>("RequestedTime"),
        tubecat = dataRow.Field<int?>("TubeCategory"),
        sname = dataRow.Field<string>("sname"),
        rnkname = dataRow.Field<string>("rnkname"),
        sno = dataRow.Field<string>("sno"),
        inililes = dataRow.Field<string>("inililes"),

        relasiont = dataRow.Field<string>("relasiondet"),
    }).ToList();



            // var lablist = (from f in db.Lab_Report.Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year) select f).GroupBy(s => new { s.Lab_SubCategory.Lab_MainCategory.CategoryName, s.PDID, s.TestSID }).Select(g => g.FirstOrDefault()).OrderBy(g => g.TestSID).Select(s => new getlabdata { pdids = s.PDID, inililes = s.Patient_Detail.Patient.Initials, sname = s.Patient_Detail.Patient.Surname, sno = s.Patient_Detail.Patient.ServiceNo, rnkname = s.Patient_Detail.Patient.rank1.RNK_NAME, catname = s.Lab_SubCategory.Lab_MainCategory.CategoryName, tsid = s.TestSID, catid = s.Lab_SubCategory.Lab_MainCategory.CategoryID, relasiont = s.Patient_Detail.Patient.RelationshipType1.Relationship, rtime = s.RequestedTime.ToString(), rtimed = s.RequestedTime,tubecat=s.Lab_SubCategory.Lab_MainCategory.TubeCategory }).OrderByDescending(p => p.rtimed).ToList();
            //db.Lab_Report.Include(l => l.Patient_Detail).Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year);
            //IEnumerable<Lab_Report> filist = lablist.GroupBy(c => new { c.Lab_SubCategory.Lab_MainCategory.CategoryName, c.Lab_SubCategory.Lab_MainCategory.CategoryID, c.PDID, c.TestSID }).Select(group => group.FirstOrDefault()).OrderByDescending(p => p.RequestedTime);
            var i1 = ""; var i2 = ""; int? i3 = 0;
            List<getlabdata> temp = new List<getlabdata>();
            List<getlabdata> temp2 = new List<getlabdata>();

            string lnm = "";
            int cvb = 0;
            foreach (var item in lablist)
            {

                if (i1.Equals(item.tsid) && i3.Equals(item.tubecat))
                {
                   if(temp.Any(x=>x.tsid==item.tsid&&x.tubecat==item.tubecat))
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    else if (temp2.Any(x=>x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp2.RemoveAt(temp2.Count - 1);
                    }
                   
                    lnm = lnm + "/" + item.catname;
                    item.catname = lnm;
                    temp2.Add(item);
                }
                else
                {
                    lnm = "";
                    cvb++;
                    temp.Add(item);

                    lnm = lnm + "/" + item.catname;
                }
                i1 = item.tsid;
                i2 = item.sno;
                i3 = item.tubecat;

            }

            var joined3 = temp.Concat(temp2).OrderByDescending(d => d.rtimed);

            int count = 0;
            foreach (var item in joined3)
            {

                dataGridView1.Rows.Add();
                dataGridView1.Rows[count].Cells["ServiceNo"].Value = item.sno;
                dataGridView1.Rows[count].Cells["Initials"].Value = item.inililes;
                dataGridView1.Rows[count].Cells["Surname"].Value = item.sname;
                dataGridView1.Rows[count].Cells["Test"].Value = item.catname;
                dataGridView1.Rows[count].Cells["pdid"].Value = item.tsid;
                dataGridView1.Rows[count].Cells["ptype"].Value = item.relasiont;
                dataGridView1.Rows[count].Cells["rqdate"].Value = item.rtime;
                count++;
            }


        }
        private void LoadPersonal3(string id)
        {
            DateTime batchdate = new DateTime();
            DataSet odsLoadLoanAppliedPersonal = new DataSet();
            odsLoadLoanAppliedPersonal.Clear();
            dataGridView1.Rows.Clear();
            DataTable oDataSetv6 = new DataTable();
            oSqlConnection = new SqlConnection(cons);
            oSqlCommand = new SqlCommand();
            sqlQuery = " SELECT max(c.initials)inililes,e.TubeCategory,a.TestSID,e.CategoryID,a.pdid,e.CategoryName,max(a.Issued) Issued,CAST(a.[RequestedTime]  as DATE) " +
 " as RequestedTime,  max(c.surname)  sname ,max(b.RNK_NAME) rnkname, max(c.ServiceNo) sno  , max(c.RelationshipType)" +
 "  relasiont, max(h.Relationship)  relasiondet FROM[MMS].[dbo] .[Lab_Report] as a with(nolock)  inner join[MMS].[dbo].[Lab_SubCategory] as d" +
" on d.[LabTestID]=a.[LabTestID] inner join[MMS].[dbo].  [Lab_MainCategory] as e on e.CategoryID=d.CategoryID inner join[MMS].[dbo]." +
"   [Patient_Detail] as l on l.pdid=a.PDID inner join[MMS].[dbo].[Patient] as c on l.pid=c.pid left join[MMS].[dbo].[RelationshipType] as h on h.RTypeID=c.RelationshipType" +
" left join[MMS].[dbo].[ranks] as b on b.RANK=c.RANK " +
" where a.RequestedLocID='" + loc + "' and a.IsPrint='0' and convert(date, a.RequestedTime)=convert(varchar,'" + dateTimePicker1.Value.Date.ToString() + "',111) group by " +
" e.CategoryName, a.TestSID, CAST(a.[RequestedTime] as DATE),e.CategoryID,e.TubeCategory,a.pdid order by e.TubeCategory, a.TestSID desc";
            // sqlQuery = " SELECT TOP 1 locdata FROM vtsdata where devid='" + devid + "' and  createddate between   '" + dt1 + "' and '" + dt2 + "'  ORDER BY vtsid DESC ";
            oSqlCommand.Connection = oSqlConnection;
            oSqlCommand.CommandText = sqlQuery;
            oSqlCommand.CommandTimeout = 120000;
            //   oSqlConnection.Open();
            oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            oSqlDataAdapter.Fill(oDataSetv6);
            // oSqlConnection.Close();
            var lablist = oDataSetv6.AsEnumerable()
    .Select(dataRow => new getlabdata
    {

        tsid = dataRow.Field<string>("TestSID"),
        catid = dataRow.Field<string>("CategoryID"),
        pdids = dataRow.Field<string>("pdid"),
        catname = dataRow.Field<string>("CategoryName"),
        rtime = dataRow.Field<DateTime?>("RequestedTime").ToString(),
        rtimed = dataRow.Field<DateTime?>("RequestedTime"),
        tubecat = dataRow.Field<int?>("TubeCategory"),
        sname = dataRow.Field<string>("sname"),
        rnkname = dataRow.Field<string>("rnkname"),
        sno = dataRow.Field<string>("sno"),
        inililes = dataRow.Field<string>("inililes"),

        relasiont = dataRow.Field<string>("relasiondet"),
    }).ToList();



            // var lablist = (from f in db.Lab_Report.Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year) select f).GroupBy(s => new { s.Lab_SubCategory.Lab_MainCategory.CategoryName, s.PDID, s.TestSID }).Select(g => g.FirstOrDefault()).OrderBy(g => g.TestSID).Select(s => new getlabdata { pdids = s.PDID, inililes = s.Patient_Detail.Patient.Initials, sname = s.Patient_Detail.Patient.Surname, sno = s.Patient_Detail.Patient.ServiceNo, rnkname = s.Patient_Detail.Patient.rank1.RNK_NAME, catname = s.Lab_SubCategory.Lab_MainCategory.CategoryName, tsid = s.TestSID, catid = s.Lab_SubCategory.Lab_MainCategory.CategoryID, relasiont = s.Patient_Detail.Patient.RelationshipType1.Relationship, rtime = s.RequestedTime.ToString(), rtimed = s.RequestedTime,tubecat=s.Lab_SubCategory.Lab_MainCategory.TubeCategory }).OrderByDescending(p => p.rtimed).ToList();
            //db.Lab_Report.Include(l => l.Patient_Detail).Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year);
            //IEnumerable<Lab_Report> filist = lablist.GroupBy(c => new { c.Lab_SubCategory.Lab_MainCategory.CategoryName, c.Lab_SubCategory.Lab_MainCategory.CategoryID, c.PDID, c.TestSID }).Select(group => group.FirstOrDefault()).OrderByDescending(p => p.RequestedTime);
            var i1 = ""; var i2 = ""; int? i3 = 0;
            List<getlabdata> temp = new List<getlabdata>();
            List<getlabdata> temp2 = new List<getlabdata>();

            string lnm = "";
            int cvb = 0;
            foreach (var item in lablist)
            {

                if (i1.Equals(item.tsid) && i3.Equals(item.tubecat))
                {
                    if (temp.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    else if (temp2.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp2.RemoveAt(temp2.Count - 1);
                    }
                    lnm = lnm + "/" + item.catname;
                    item.catname = lnm;
                    temp2.Add(item);
                }
                else
                {
                    lnm = "";
                    cvb++;
                    temp.Add(item);

                    lnm = lnm + "/" + item.catname;
                }
                i1 = item.tsid;
                i2 = item.sno;
                i3 = item.tubecat;

            }

            var joined3 = temp.Concat(temp2).OrderByDescending(d => d.rtimed);

            int count = 0;
            foreach (var item in joined3)
            {

                dataGridView1.Rows.Add();
                dataGridView1.Rows[count].Cells["ServiceNo"].Value = item.sno;
                dataGridView1.Rows[count].Cells["Initials"].Value = item.inililes;
                dataGridView1.Rows[count].Cells["Surname"].Value = item.sname;
                dataGridView1.Rows[count].Cells["Test"].Value = item.catname;
                dataGridView1.Rows[count].Cells["pdid"].Value = item.tsid;
                dataGridView1.Rows[count].Cells["ptype"].Value = item.relasiont;
                dataGridView1.Rows[count].Cells["rqdate"].Value = item.rtime;
                count++;
            }



        }
        private void LoadPersonal4(string id)
        {
            DateTime batchdate = new DateTime();
            DataSet odsLoadLoanAppliedPersonal = new DataSet();
            odsLoadLoanAppliedPersonal.Clear();
            dataGridView1.Rows.Clear();
            DataTable oDataSetv6 = new DataTable();
            oSqlConnection = new SqlConnection(cons);
            oSqlCommand = new SqlCommand();
            sqlQuery = " SELECT e.TubeCategory,a.TestSID,e.CategoryID,a.pdid,e.CategoryName,max(a.Issued) Issued,CAST(a.[RequestedTime] " +
" as DATE) as RequestedTime, COALESCE(NULLIF(concat(max(case when c.RelationshipType = 1 and b.Surname != '0' " +
" then b.Surname end), max(case when c.RelationshipType = 2 then k.SpouseName end), " +
" max(case when c.RelationshipType = 5 and c.DateOfBirth = f.DOB then f.ChildName end), " +
" max(case when c.RelationshipType = 3 and g.Relationship = 'Father' then g.ParentName end), " +
" max(case when c.RelationshipType = 4 and g.Relationship = 'Mother' then g.ParentName end)), ''), max(c.surname)) " +
" sname ,max(case when c.RelationshipType = 1 then b.Rank end) rnkname, max(c.ServiceNo) sno " +
" ,max(case when c.RelationshipType = 1 then b.Initials end) inililes, max(c.RelationshipType) relasiont, max(h.Relationship) " +
                " relasiondet FROM[MMS].[dbo] .[Lab_Report] as a with(nolock) " +
" inner join[MMS].[dbo].[Lab_SubCategory] as d on d.[LabTestID]=a.[LabTestID] inner join[MMS].[dbo]. " +
" [Lab_MainCategory] as e on e.CategoryID=d.CategoryID " +
" inner join[MMS].[dbo].[Patient_Detail] as l on l.pdid=a.PDID inner join[MMS].[dbo].[Patient] as c on l.pid=c.pid " +
" left join[MMS].[dbo].[PersonalDetails] as b on c.ServiceNo=b.ServiceNo " +
" left join[MMS].[dbo].[SpouseDetails] as k on b.SNo=k.SNo " +
" left join[MMS].[dbo].[Children] as f on b.SNo=f.SNo left join[MMS].[dbo].[parents] as g on b.SNo=g.SNo " +
" left join[MMS].[dbo].[RelationshipType] as h on h.RTypeID=c.RelationshipType " +
" where a.RequestedLocID='" + loc + "' and c.serviceno='" + id + "' and a.IsPrint='0' and convert(date, a.RequestedTime)=convert(varchar,'" + dateTimePicker1.Value.Date.ToString() + "',111) group by " +
" e.CategoryName, a.TestSID, CAST(a.[RequestedTime] as DATE),e.CategoryID,e.TubeCategory,a.pdid order by e.TubeCategory, a.TestSID desc";
            // sqlQuery = " SELECT TOP 1 locdata FROM vtsdata where devid='" + devid + "' and  createddate between   '" + dt1 + "' and '" + dt2 + "'  ORDER BY vtsid DESC ";
            oSqlCommand.Connection = oSqlConnection;
            oSqlCommand.CommandText = sqlQuery;
            oSqlCommand.CommandTimeout = 120000;
            //   oSqlConnection.Open();
            oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            oSqlDataAdapter.Fill(oDataSetv6);
            // oSqlConnection.Close();
            var lablist = oDataSetv6.AsEnumerable()
    .Select(dataRow => new getlabdata
    {

        tsid = dataRow.Field<string>("TestSID"),
        catid = dataRow.Field<string>("CategoryID"),
        pdids = dataRow.Field<string>("pdid"),
        catname = dataRow.Field<string>("CategoryName"),
        rtime = dataRow.Field<DateTime?>("RequestedTime").ToString(),
        rtimed = dataRow.Field<DateTime?>("RequestedTime"),
        tubecat = dataRow.Field<int?>("TubeCategory"),
        sname = dataRow.Field<string>("sname"),
        rnkname = dataRow.Field<string>("rnkname"),
        sno = dataRow.Field<string>("sno"),
        inililes = dataRow.Field<string>("inililes"),

        relasiont = dataRow.Field<string>("relasiondet"),
    }).ToList();



            // var lablist = (from f in db.Lab_Report.Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year) select f).GroupBy(s => new { s.Lab_SubCategory.Lab_MainCategory.CategoryName, s.PDID, s.TestSID }).Select(g => g.FirstOrDefault()).OrderBy(g => g.TestSID).Select(s => new getlabdata { pdids = s.PDID, inililes = s.Patient_Detail.Patient.Initials, sname = s.Patient_Detail.Patient.Surname, sno = s.Patient_Detail.Patient.ServiceNo, rnkname = s.Patient_Detail.Patient.rank1.RNK_NAME, catname = s.Lab_SubCategory.Lab_MainCategory.CategoryName, tsid = s.TestSID, catid = s.Lab_SubCategory.Lab_MainCategory.CategoryID, relasiont = s.Patient_Detail.Patient.RelationshipType1.Relationship, rtime = s.RequestedTime.ToString(), rtimed = s.RequestedTime,tubecat=s.Lab_SubCategory.Lab_MainCategory.TubeCategory }).OrderByDescending(p => p.rtimed).ToList();
            //db.Lab_Report.Include(l => l.Patient_Detail).Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year);
            //IEnumerable<Lab_Report> filist = lablist.GroupBy(c => new { c.Lab_SubCategory.Lab_MainCategory.CategoryName, c.Lab_SubCategory.Lab_MainCategory.CategoryID, c.PDID, c.TestSID }).Select(group => group.FirstOrDefault()).OrderByDescending(p => p.RequestedTime);
            var i1 = ""; var i2 = ""; int? i3 = 0;
            List<getlabdata> temp = new List<getlabdata>();
            List<getlabdata> temp2 = new List<getlabdata>();

            string lnm = "";
            int cvb = 0;
            foreach (var item in lablist)
            {

                if (i1.Equals(item.tsid) && i3.Equals(item.tubecat))
                {
                    if (temp.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    else if (temp2.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp2.RemoveAt(temp2.Count - 1);
                    }
                    lnm = lnm + "/" + item.catname;
                    item.catname = lnm;
                    temp2.Add(item);
                }
                else
                {
                    lnm = "";
                    cvb++;
                    temp.Add(item);

                    lnm = lnm + "/" + item.catname;
                }
                i1 = item.tsid;
                i2 = item.sno;
                i3 = item.tubecat;

            }

            var joined3 = temp.Concat(temp2).OrderByDescending(d => d.rtimed);

            int count = 0;
            foreach (var item in joined3)
            {

                dataGridView1.Rows.Add();
                dataGridView1.Rows[count].Cells["ServiceNo"].Value = item.sno;
                dataGridView1.Rows[count].Cells["Initials"].Value = item.inililes;
                dataGridView1.Rows[count].Cells["Surname"].Value = item.sname;
                dataGridView1.Rows[count].Cells["Test"].Value = item.catname;
                dataGridView1.Rows[count].Cells["pdid"].Value = item.tsid;
                dataGridView1.Rows[count].Cells["ptype"].Value = item.relasiont;
                dataGridView1.Rows[count].Cells["rqdate"].Value = item.rtime;
                count++;
            }


        }
        private void LoadPersonal5(string id)
        {
            DateTime batchdate = new DateTime();
            DataSet odsLoadLoanAppliedPersonal = new DataSet();
            odsLoadLoanAppliedPersonal.Clear();
            dataGridView1.Rows.Clear();
            DataTable oDataSetv6 = new DataTable();
            oSqlConnection = new SqlConnection(cons);
            oSqlCommand = new SqlCommand();
            sqlQuery = " SELECT max(c.initials)inililes,e.TubeCategory,a.TestSID,e.CategoryID,a.pdid,e.CategoryName,max(a.Issued) Issued,CAST(a.[RequestedTime]  as DATE) " +
 " as RequestedTime,  max(c.surname)  sname ,max(b.RNK_NAME) rnkname, max(c.ServiceNo) sno  , max(c.RelationshipType)" +
 "  relasiont, max(h.Relationship)  relasiondet FROM[MMS].[dbo] .[Lab_Report] as a with(nolock)  inner join[MMS].[dbo].[Lab_SubCategory] as d" +
" on d.[LabTestID]=a.[LabTestID] inner join[MMS].[dbo].  [Lab_MainCategory] as e on e.CategoryID=d.CategoryID inner join[MMS].[dbo]." +
"   [Patient_Detail] as l on l.pdid=a.PDID inner join[MMS].[dbo].[Patient] as c on l.pid=c.pid left join[MMS].[dbo].[RelationshipType] as h on h.RTypeID=c.RelationshipType" +
" left join[MMS].[dbo].[ranks] as b on b.RANK=c.RANK " +
" where a.RequestedLocID='" + loc + "' and a.IsPrint='1' and convert(date, a.RequestedTime)=convert(varchar,'" + dateTimePicker1.Value.Date.ToString() + "',111) group by " +
" e.CategoryName, a.TestSID, CAST(a.[RequestedTime] as DATE),e.CategoryID,e.TubeCategory,a.pdid order by e.TubeCategory, a.TestSID desc";
            // sqlQuery = " SELECT TOP 1 locdata FROM vtsdata where devid='" + devid + "' and  createddate between   '" + dt1 + "' and '" + dt2 + "'  ORDER BY vtsid DESC ";
            oSqlCommand.Connection = oSqlConnection;
            oSqlCommand.CommandText = sqlQuery;
            oSqlCommand.CommandTimeout = 120000;
            //   oSqlConnection.Open();
            oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            oSqlDataAdapter.Fill(oDataSetv6);
            // oSqlConnection.Close();
            var lablist = oDataSetv6.AsEnumerable()
    .Select(dataRow => new getlabdata
    {

        tsid = dataRow.Field<string>("TestSID"),
        catid = dataRow.Field<string>("CategoryID"),
        pdids = dataRow.Field<string>("pdid"),
        catname = dataRow.Field<string>("CategoryName"),
        rtime = dataRow.Field<DateTime?>("RequestedTime").ToString(),
        rtimed = dataRow.Field<DateTime?>("RequestedTime"),
        tubecat = dataRow.Field<int?>("TubeCategory"),
        sname = dataRow.Field<string>("sname"),
        rnkname = dataRow.Field<string>("rnkname"),
        sno = dataRow.Field<string>("sno"),
        inililes = dataRow.Field<string>("inililes"),

        relasiont = dataRow.Field<string>("relasiondet"),
    }).ToList();



            // var lablist = (from f in db.Lab_Report.Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year) select f).GroupBy(s => new { s.Lab_SubCategory.Lab_MainCategory.CategoryName, s.PDID, s.TestSID }).Select(g => g.FirstOrDefault()).OrderBy(g => g.TestSID).Select(s => new getlabdata { pdids = s.PDID, inililes = s.Patient_Detail.Patient.Initials, sname = s.Patient_Detail.Patient.Surname, sno = s.Patient_Detail.Patient.ServiceNo, rnkname = s.Patient_Detail.Patient.rank1.RNK_NAME, catname = s.Lab_SubCategory.Lab_MainCategory.CategoryName, tsid = s.TestSID, catid = s.Lab_SubCategory.Lab_MainCategory.CategoryID, relasiont = s.Patient_Detail.Patient.RelationshipType1.Relationship, rtime = s.RequestedTime.ToString(), rtimed = s.RequestedTime,tubecat=s.Lab_SubCategory.Lab_MainCategory.TubeCategory }).OrderByDescending(p => p.rtimed).ToList();
            //db.Lab_Report.Include(l => l.Patient_Detail).Where(p => p.RequestedLocID == locid).Where(p => p.Issued == "0").Where(p => p.RequestedTime.Value.Day == dt1.Day && p.RequestedTime.Value.Month == dt1.Month && p.RequestedTime.Value.Year == dt1.Year);
            //IEnumerable<Lab_Report> filist = lablist.GroupBy(c => new { c.Lab_SubCategory.Lab_MainCategory.CategoryName, c.Lab_SubCategory.Lab_MainCategory.CategoryID, c.PDID, c.TestSID }).Select(group => group.FirstOrDefault()).OrderByDescending(p => p.RequestedTime);
            var i1 = ""; var i2 = ""; int? i3 = 0;
            List<getlabdata> temp = new List<getlabdata>();
            List<getlabdata> temp2 = new List<getlabdata>();

            string lnm = "";
            int cvb = 0;
            foreach (var item in lablist)
            {

                if (i1.Equals(item.tsid) && i3.Equals(item.tubecat))
                {
                    if (temp.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    else if (temp2.Any(x => x.tsid == item.tsid && x.tubecat == item.tubecat))
                    {
                        temp2.RemoveAt(temp2.Count - 1);
                    }
                    lnm = lnm + "/" + item.catname;
                    item.catname = lnm;
                    temp2.Add(item);
                }
                else
                {
                    lnm = "";
                    cvb++;
                    temp.Add(item);

                    lnm = lnm + "/" + item.catname;
                }
                i1 = item.tsid;
                i2 = item.sno;
                i3 = item.tubecat;

            }

            var joined3 = temp.Concat(temp2).OrderByDescending(d => d.rtimed);

            int count = 0;
            foreach (var item in joined3)
            {

                dataGridView1.Rows.Add();
                dataGridView1.Rows[count].Cells["ServiceNo"].Value = item.sno;
                dataGridView1.Rows[count].Cells["Initials"].Value = item.inililes;
                dataGridView1.Rows[count].Cells["Surname"].Value = item.sname;
                dataGridView1.Rows[count].Cells["Test"].Value = item.catname;
                dataGridView1.Rows[count].Cells["pdid"].Value = item.tsid;
                dataGridView1.Rows[count].Cells["ptype"].Value = item.relasiont;
                dataGridView1.Rows[count].Cells["rqdate"].Value = item.rtime;
                count++;
            }



        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPersonal();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
           
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
          Bitmap bmp=new  Bitmap(this.label2.Width, this.label2.Height);
         this.label2.DrawToBitmap(bmp, new Rectangle(0, 0, this.label2.Width, this.label2.Height));
            Image wer = pictureBox1.Image;
            var btmap = new Bitmap(Math.Max(wer.Width, bmp.Width), (wer.Height + bmp.Height));
            using(Graphics g = Graphics.FromImage(btmap))
            {
                g.DrawImage(wer, 0, 0);
                g.DrawImage(bmp, 15, wer.Height);
              
            }
            e.Graphics.DrawImage(btmap, 0, 0);
            //Graphics g = Graphics.FromImage(wer);
            //g.DrawImage(bmp, (pictureBox1.Image.Width / 2)+20, (wer.Height-10));

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                try
                {
                    var senderGrid = (DataGridView)sender;
                    if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        
                        string sdf = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["pdid"].Value.ToString();
                        string lbl2 = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Test"].Value.ToString();
                        BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
                        {
                            IncludeLabel = true,
                            Alignment = AlignmentPositions.CENTER,
                            LabelPosition=LabelPositions.BOTTOMCENTER,
                            Width = 250,
                            Height = 50,
                            RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                            BackColor = Color.White,
                            ForeColor = Color.Black,
                        };

                        Image img = barcode.Encode(TYPE.CODE128, sdf);

                        pictureBox1.Image = img;
                        this.label2.Text = lbl2;
                        //PrintDocument printDocument1 = new PrintDocument();
                        //printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                        //printDocument1.Print();
                        TSCLIB_DLL.openport("TSC TTP-244 Pro");                                           //Open specified printer driver
                        TSCLIB_DLL.setup("48", "20", "4", "8", "0", "1", "10");                           //Setup the media size and sensor type info
                        TSCLIB_DLL.clearbuffer();                                                           //Clear image buffer
                        TSCLIB_DLL.barcode("80", "50", "128", "100", "1", "0", "2", "2", sdf); //Drawing barcode
                        TSCLIB_DLL.printerfont("80", "25", "3", "0", "1", "1",sdf);
                        TSCLIB_DLL.printerfont("80", "0", "3", "0", "1", "1", lbl2);
                        // TSCLIB_DLL.windowsfont(100, 300, 24, 0, 0, 0, "ARIAL", "Windows Arial Font Test");  //Draw windows font
                        //TSCLIB_DLL.downloadpcx("C:\\ASP.NET_in_VCsharp_2008\\ASP.NET_in_VCsharp_2008\\UL.PCX", "UL.PCX");                                         //Download PCX file into printer
                        // TSCLIB_DLL.downloadpcx("UL.PCX", "UL.PCX");                                         //Download PCX file into printer
                        // TSCLIB_DLL.sendcommand("PUTPCX 100,400,\"UL.PCX\"");                                //Drawing PCX graphic
                        TSCLIB_DLL.printlabel("1", "1");                                                    //Print labels
                        TSCLIB_DLL.closeport();



                        String strDBCon = cons;
                        oSqlConnection = new SqlConnection(strDBCon);
                        oSqlCommand = new SqlCommand();
                        sqlQuery = "update [MMS].[dbo].[Lab_Report] set IsPrint='1'  where TestSID='"+sdf+"'   ";

                        oSqlCommand.Connection = oSqlConnection;
                        oSqlCommand.CommandText = sqlQuery;

                        oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
                        oSqlConnection.Open();
                        oSqlCommand.ExecuteNonQuery();
                        oSqlConnection.Close();
                        dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadPersonal2(this.textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadPersonal();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadPersonal3(this.textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TSCLIB_DLL.openport("TSC TTP-244 Pro");                                           //Open specified printer driver
            TSCLIB_DLL.setup("48", "20", "4", "8", "0", "1", "10");                           //Setup the media size and sensor type info
            TSCLIB_DLL.clearbuffer();                                                           //Clear image buffer
            TSCLIB_DLL.barcode("100", "50", "128", "100", "1", "0", "2", "2", "036141110x1"); //Drawing barcode
            TSCLIB_DLL.printerfont("100", "25", "3", "0", "1", "1", "036141110x1");
            TSCLIB_DLL.printerfont("100", "0", "3", "0", "1", "1", "FBC");
            // TSCLIB_DLL.windowsfont(100, 300, 24, 0, 0, 0, "ARIAL", "Windows Arial Font Test");  //Draw windows font
            //TSCLIB_DLL.downloadpcx("C:\\ASP.NET_in_VCsharp_2008\\ASP.NET_in_VCsharp_2008\\UL.PCX", "UL.PCX");                                         //Download PCX file into printer
            // TSCLIB_DLL.downloadpcx("UL.PCX", "UL.PCX");                                         //Download PCX file into printer
            // TSCLIB_DLL.sendcommand("PUTPCX 100,400,\"UL.PCX\"");                                //Drawing PCX graphic
            TSCLIB_DLL.printlabel("1", "1");                                                    //Print labels
            TSCLIB_DLL.closeport();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadPersonal2(this.textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            LoadPersonal5(this.textBox1.Text);
        }
    }


}
