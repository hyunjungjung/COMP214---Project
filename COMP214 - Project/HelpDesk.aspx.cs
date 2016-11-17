﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Oracle.DataAccess.Client;

public partial class HelpDesk : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      // Define data objects
      OracleConnection conn;
      OracleCommand categoryComm;
      OracleCommand subjectComm;
      OracleDataReader reader;
      // Read the connection string from Web.config
      string connectionString =
          ConfigurationManager.ConnectionStrings[
          "Dorknozzle"].ConnectionString;
      // Initialize connection
      conn = new OracleConnection(connectionString);
      // Create command to read the help desk categories
      categoryComm = new OracleCommand(
          "SELECT \"CategoryID\", \"Category\" FROM \"HelpDeskCategories\"",
          conn);
      // Create command to read the help desk subjects
      subjectComm = new OracleCommand(
          "SELECT \"SubjectID\", \"Subject\" FROM \"HelpDeskSubjects\"", conn);
      // Enclose database code in Try-Catch-Finally
      try
      {
        // Open the connection
        conn.Open();
        // Execute the category command
        reader = categoryComm.ExecuteReader();
        // Populate the list of categories
        categoryList.DataSource = reader;
        categoryList.DataValueField = "CategoryID";
        categoryList.DataTextField = "Category";
        categoryList.DataBind();
        // Close the reader
        reader.Close();
        // Execute the subjects command
        reader = subjectComm.ExecuteReader();
        // Populate the list of subjects
        subjectList.DataSource = reader;
        subjectList.DataValueField = "SubjectID";
        subjectList.DataTextField = "Subject";
        subjectList.DataBind();
        // Close the reader
        reader.Close();
      }
      finally
      {
        // Close the connection
        conn.Close();
      }
    }
  }
  protected void submitButton_Click(object sender, EventArgs e)
  {
    if (Page.IsValid)
    {
      // Define data objects
      OracleConnection conn;
      OracleCommand comm;
      // Read the connection string from Web.config
      string connectionString =
          ConfigurationManager.ConnectionStrings[
          "Dorknozzle"].ConnectionString;
      // Initialize connection
      conn = new OracleConnection(connectionString);
      // Create command 
      comm = new OracleCommand("InsertHelpDesk", conn);
      // Specify we're calling a stored procedure
      comm.CommandType = System.Data.CommandType.StoredProcedure;
      // Add command parameters
      comm.Parameters.Add("@EmployeeID", OracleDbType.Int32);
      comm.Parameters["@EmployeeID"].Value = 5;
      comm.Parameters.Add("@StationNumber", OracleDbType.Int32);
      comm.Parameters["@StationNumber"].Value = stationTextBox.Text;
      comm.Parameters.Add("@CategoryID", OracleDbType.Int32);
      comm.Parameters["@CategoryID"].Value =
          categoryList.SelectedItem.Value;
      comm.Parameters.Add("@SubjectID", OracleDbType.Int32);
      comm.Parameters["@SubjectID"].Value =
          subjectList.SelectedItem.Value;
      comm.Parameters.Add("@Description",
          OracleDbType.NVarchar2, 50);
      comm.Parameters["@Description"].Value =
          descriptionTextBox.Text;
      comm.Parameters.Add("@StatusID", OracleDbType.Int32);
      comm.Parameters["@StatusID"].Value = 1;
      // Enclose database code in Try-Catch-Finally
      try
      {
        // Open the connection
        conn.Open();
        // Execute the command
        comm.ExecuteNonQuery();
        // Reload page if the query executed successfully
        Response.Redirect("HelpDesk.aspx");
      }
      catch
      {
        // Display error message
        dbErrorMessage.Text =
            "Error submitting the help desk request! Please " +
            "try again later, and/or change the entered data!";
      }
      finally
      {
        // Close the connection
        conn.Close();
      }
    }
  }
}
