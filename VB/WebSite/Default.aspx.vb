Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

Partial Public Class Grid_Editing_ModifyKeyFieldValueWhenUsingCompositeKey
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

	End Sub

	Protected Sub ASPxGridView1_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewColumnDataEventArgs)
		If e.Column.FieldName = "CompositeKey" Then
			'Compose a primary key value
			Dim firstName As String = Convert.ToString(e.GetListSourceFieldValue("FirstName"))
			Dim lastName As String = Convert.ToString(e.GetListSourceFieldValue("LastName"))
			Dim birthDate As String = Convert.ToString(e.GetListSourceFieldValue("BirthDate"))
			e.Value = firstName & "-" & lastName & "-" & birthDate
		End If
	End Sub

	Protected Sub ASPxGridView1_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)
		'Create a connection to a data source
		Dim dataFileName As String = Me.MapPath("..\..\..\App_Data\nwind.mdb")
		Dim connection As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dataFileName)

		'Define an update command text
		Dim updateCommandText As String = "UPDATE Employees SET TitleOfCourtesy = @TitleOfCourtesy, FirstName = @FirstName, LastName = @LastName, City = @City WHERE FirstName = @oldFirstName AND LastName = @oldLastName AND BirthDate = @oldBirthDate"

		'Create an update command object
		Dim updateCommand As OleDbCommand = New OleDbCommand(updateCommandText, connection)

		'Create the update command's required parameters with their values
		updateCommand.Parameters.AddWithValue("@TitleOfCourtesy", Convert.ToString(e.NewValues("TitleOfCourtesy")))
		updateCommand.Parameters.AddWithValue("@FirstName", Convert.ToString(e.NewValues("FirstName")))
		updateCommand.Parameters.AddWithValue("@LastName", Convert.ToString(e.NewValues("LastName")))
		updateCommand.Parameters.AddWithValue("@City", Convert.ToString(e.NewValues("City")))
		updateCommand.Parameters.AddWithValue("@oldFirstName", Convert.ToString(e.OldValues("FirstName"))) 'Key field
		updateCommand.Parameters.AddWithValue("@oldLastName", Convert.ToString(e.OldValues("LastName"))) 'Key field
		updateCommand.Parameters.AddWithValue("@oldBirthDate", Convert.ToDateTime(e.OldValues("BirthDate"))) 'Key field

		Try
			'Execute the update command
			updateCommand.Connection.Open()
			updateCommand.ExecuteNonQuery()
		Catch
			updateCommand.Connection.Close()
			Throw New InvalidOperationException("Data modifications are not allowed in the online demo")
		Finally
			'Cancel the ASPxGridView default update processing 
			e.Cancel = True
			'Switch the ASPxGridView to browse mode
			ASPxGridView1.CancelEdit()
		End Try
	End Sub

	Protected Sub CheckBox1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
		ASPxGridView1.Columns("CompositeKey").Visible = (TryCast(sender, CheckBox)).Checked
	End Sub
End Class
