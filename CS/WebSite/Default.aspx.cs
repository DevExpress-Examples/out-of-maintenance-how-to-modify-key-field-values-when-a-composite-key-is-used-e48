using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Grid_Editing_ModifyKeyFieldValueWhenUsingCompositeKey : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e){

    }

    protected void ASPxGridView1_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewColumnDataEventArgs e) {
        if (e.Column.FieldName == "CompositeKey") {
            //Compose a primary key value
            string firstName = Convert.ToString(e.GetListSourceFieldValue("FirstName"));
            string lastName = Convert.ToString(e.GetListSourceFieldValue("LastName"));
            string birthDate = Convert.ToString(e.GetListSourceFieldValue("BirthDate"));
            e.Value = firstName + "-" + lastName + "-" + birthDate;
        }
    }

    protected void ASPxGridView1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e) {
        //Create a connection to a data source
        String dataFileName = this.MapPath("..\\..\\..\\App_Data\\nwind.mdb");
        OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataFileName);

        //Define an update command text
        string updateCommandText = "UPDATE Employees SET TitleOfCourtesy = @TitleOfCourtesy, FirstName = @FirstName, LastName = @LastName, City = @City WHERE FirstName = @oldFirstName AND LastName = @oldLastName AND BirthDate = @oldBirthDate";

        //Create an update command object
        OleDbCommand updateCommand = new OleDbCommand(updateCommandText, connection);

        //Create the update command's required parameters with their values
        updateCommand.Parameters.AddWithValue("@TitleOfCourtesy", Convert.ToString(e.NewValues["TitleOfCourtesy"]));
        updateCommand.Parameters.AddWithValue("@FirstName", Convert.ToString(e.NewValues["FirstName"]));
        updateCommand.Parameters.AddWithValue("@LastName", Convert.ToString(e.NewValues["LastName"]));
        updateCommand.Parameters.AddWithValue("@City", Convert.ToString(e.NewValues["City"]));
        updateCommand.Parameters.AddWithValue("@oldFirstName", Convert.ToString(e.OldValues["FirstName"]));   //Key field
        updateCommand.Parameters.AddWithValue("@oldLastName", Convert.ToString(e.OldValues["LastName"]));     //Key field
        updateCommand.Parameters.AddWithValue("@oldBirthDate", Convert.ToDateTime(e.OldValues["BirthDate"])); //Key field

        try {
            //Execute the update command
            updateCommand.Connection.Open();
            updateCommand.ExecuteNonQuery();
        }
        catch {
            updateCommand.Connection.Close();
            throw new InvalidOperationException("Data modifications are not allowed in the online demo");
        }
        finally {
            //Cancel the ASPxGridView default update processing 
            e.Cancel = true;
            //Switch the ASPxGridView to browse mode
            ASPxGridView1.CancelEdit();
        }
    }

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e) {
        ASPxGridView1.Columns["CompositeKey"].Visible = (sender as CheckBox).Checked;
    }
}
