<%@ Page Title="" Language="C#" MasterPageFile="~/MainSite.Master" AutoEventWireup="true" CodeBehind="ProjectMaster.aspx.cs" Inherits="PMS.ProjectMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script src="js/plugins/xeditable/demo.js"></script>--%>
    <%-- XEDITABLE IS THE JQUERY PLUGIN WHICH IS USED FOR INLINE EDIT --%>
    <script src="js/plugins/xeditable/bootstrap-editable.min.js"></script>
    <link href="js/plugins/xeditable/bootstrap-editable.css" rel="stylesheet" />
    <script src="js/plugins/xeditable/address.js"></script>
    <style type="text/css" class="init">
        td.details-control {
            background: url(img/details_open.png) no-repeat center center;
            cursor: pointer;
        }
         
        tr.shown td.details-control {
            background: url(img/details_close.png) no-repeat center center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <input type="hidden" id="hdnLink" />
    <input type="hidden" id="hdnStatus" />

    <div class="content-header">
        <div class="row">
            <div class="col-sm-6">
                <div class="header-section">
                    <h1>Project List</h1>
                </div>
            </div>
        </div>
    </div>

    <%-- FILTER SECTION  --%>
    <div class="row">
        <div class="block full">
            <div class="tab-content">
                <div class="tab-pane active" id="tabCreate">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="form-group pull-right">
                                <a href="iframeProjectMaster.aspx" role="button" rel="tooltip" id="btnNewProject" title="" class="fancybox fancybox.iframe btn btn-primary NewCustomer"
                                    data-toggle="modal" data-original-title="Add New Record"><i class="fa fa-plus"></i>
                                    Create New Project</a>
                                <%--<a class="fancybox fancybox.iframe btn btn-primary" href="iframeProjectMaster.aspx"><i class="fa fa-plus"></i>Create New Project</a>--%>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <div class="form-group">
                                <b>Customer Name</b>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <b>User Name</b>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <b>Status</b>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <b>Start Date</b>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <b>End Date</b>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <div class="form-group">
                                <select id="ddlcustomername" name="ddlcustomername" class="select-select2" multiple placeholder="Customer Name"
                                    style="width: 100%;" size="1">
                                </select>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <select id="ddlusername" name="ddlusername" class="select-select2" multiple placeholder="User"
                                    style="width: 100%;" size="1">
                                </select>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <select id="ddlstatus" name="ddlstatus" class="select-select2" multiple placeholder="Status"
                                    style="width: 100%;" size="1">
                                    <option value="C">CLOSED</option>
                                    <option value="E">ESTIMITING</option>
                                    <option value="I">IN PROGRESS</option>
                                    <option value="N">NEW</option>
                                    <option value="T">TO MEASURE</option>
                                    <option value="W">WAITING</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <input type="text" name="startdate" readonly id="startdate" class="form-control input-datepicker"
                                    data-date-format="mm/dd/yyyy" />
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="form-group">
                                <input type="text" name="enddate" readonly id="enddate" class="form-control input-datepicker"
                                    data-date-format="mm/dd/yyyy" />
                            </div>
                        </div>
                        <div class="col-sm-1">
                            <div class="form-group">
                                <button type="button" id="Btnfilter" class="btn btn-primary">Filter</button>
                            </div>
                        </div>
                        <div class="col-sm-1">
                            <div class="form-group">
                                <button type="button" id="Btnfilterclear" class="btn btn-primary">Clear</button>
                            </div>
                        </div>

                    </div>
                    <div class="row">
                        <div class="form-group pull-right" style="padding-right: 5%">
                            <label class="csscheckbox csscheckbox-primary">
                                <input type="checkbox" id="chkClosed"><span></span>
                                CLOSED
                            </label>
                        </div>
                    </div>
                    <%-- PROJECT LIST IN WHICH USER CAN DID A EDIT AND DELETE AND INLINE EDIT OPTION AND EXPAND PROJECT DETAIL --%>
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="form-group">
                                <div id="tblContainer" style="overflow-x: auto; width: 100%;">
                                    <table class="dataTable" id="ProjectList">
                                    </table>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <div class="tab-pane" id="tabView">
                    <div id="tblContainer1" style="overflow-x: auto; width: 100%;">
                        <table class="dataTable" id="CategoryList">
                        </table>
                    </div>
                </div>
                <div id="tblItemList" style="display: none; width: 80%">
                </div>
            </div>
        </div>
    </div>

    <a class="btn btn-primary" id="btnProjectmdl" style="display: none" data-toggle="modal" href="#mdlProject">Launch Modal</a>

    <%-- CREATE NEW PROJECT USING POPUP  --%>
    <div id="mdlProject" class="modal fade" aria-hidden="true" aria-labelledby="myModalLabel" role="dialog" tabindex="-1">
        <div class="modal-dialog" style="width: 80%;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" id="btnProjectClose" aria-hidden="true">&times;</button>
                    <h3 class="modal-title"><strong id="myModalLabel">Create New Project</strong></h3>
                </div>
                <div class="modal-body" style="height: 100%">
                    <form class="form-horizontal form-validate" id="frmProjectMaster">
                        <div class="form-group">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="ddlCustomer">Customer Name*</label>
                                    <div class="col-sm-7">
                                        <select id="ddlCustomer" name="Customer" data-rule-required="true" class="select-select2" style="width: 100%;" data-placeholder="Select Customer">
                                            <option></option>
                                        </select>
                                    </div>
                                    <div class="col-sm-1" style="padding-left: 0">
                                        <a href="#" role="button" id="btnNewCustomer" class="btn btn-primary" data-toggle="modal"><i class="fa fa-plus"></i></a>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtProjectId">Temp Project No</label>
                                    <div class="col-sm-7">
                                        <input type="text" name="ProjectId" data-rule-required="true" readonly id="txtProjectId" class="form-control" placeholder="Temp Project No" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtProjectName">Project Name*</label>
                                    <div class="col-sm-7">
                                        <input type="text" name="ProjectName" data-rule-required="true" id="txtProjectName" class="form-control" placeholder="Project Name" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtDescription">Description</label>
                                    <div class="col-sm-7">
                                        <textarea name="Description" id="txtDescription" class="form-control" placeholder="Description.."></textarea>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtprojectEmail">
                                        Email</label>
                                    <div class="col-sm-7">
                                        <input type="text" name="email" data-rule-email="true" id="txtprojectEmail" class="form-control"
                                            placeholder="Enter Email" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="ddlUser">Assigned User*</label>
                                    <div class="col-sm-7">
                                        <select id="ddlUser" name="user" data-rule-required="true" class="select-select2" style="width: 100%;" data-placeholder="Select User">
                                            <option></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtDate">
                                        Kick Off Date*</label>
                                    <div class="col-sm-7">
                                        <input type="text" id="txtDate" data-rule-required="true" name="KickOffDate" readonly class="form-control input-datepicker"
                                            data-date-format="mm/dd/yyyy" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtInitialReq">Initial Requirement</label>
                                    <div class="col-sm-7">
                                        <textarea name="InitialRequirement" id="txtInitialReq" class="form-control" placeholder="Initial Requirement"></textarea>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="ddlProjectStatus">Status</label>
                                    <div class="col-sm-7">
                                        <select id="ddlProjectStatus" data-rule-required="true" name="status" class="select-select2" style="width: 100%;" data-placeholder="Select Status">
                                            <option value="T">TO MEASURE</option>
                                            <option value="W">WAITING</option>
                                            <option value="E">ESTIMITING</option>
                                            <option value="N">NEW</option>
                                            <option value="I">IN PROGRESS</option>
                                            <option value="C">CLOSED</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="ddlUser2">Sales Representative</label>
                                    <div class="col-sm-7">
                                        <select id="ddlUser2" name="user2" class="select-select2" style="width: 100%;" data-placeholder="Sales Representative">
                                            <option></option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtLatitude">Contact 1</label>
                                    <div class="col-sm-4">
                                        <input type="text" id="txtCon1Name" name="txtCon1Name" placeholder="Person Name" class="form-control Capitalize" />
                                    </div>
                                    <div class="col-sm-4">
                                        <input type="text" id="txtCon1Phone" name="txtCon1Phone" placeholder="Phone Number" class="form-control mask_phone" onkeypress="return isNumberKey(event)" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtLatitude">Contact 2</label>
                                    <div class="col-sm-4">
                                        <input type="text" id="txtCon2Name" name="txtCon2Name" placeholder="Person Name" class="form-control Capitalize" />
                                    </div>
                                    <div class="col-sm-4">
                                        <input type="text" id="txtCon2Phone" name="txtCon2Phone" placeholder="Phone Number" class="form-control mask_phone" onkeypress="return isNumberKey(event)" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtUnitNo">Unit No.</label>
                                    <div class="col-sm-7">
                                        <input type="text" name="Unit No" id="txtUnitNo" class="form-control" placeholder="Unit Number" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtAddress">Address *</label>
                                    <div class="col-sm-7">
                                        <input placeholder="Enter your address" class="form-control"
                                            onfocus="geolocate()" type="text" id="txtAddress"></input>
                                        <%--<textarea class="form-control googleAddress" onfocus="geolocate()" id="txtAddress" data-rule-required="true" name="Address"></textarea>--%>
                                    </div>
                                </div>
                                <div style="display: none">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label" for="txtLatitude">Latitude *</label>
                                        <div class="col-sm-7">
                                            <input type="text" id="txtLatitude" name="latitude" placeholder="Latitude" class="form-control auto" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label" for="txtLongitude">Longitude *</label>
                                        <div class="col-sm-7">
                                            <input type="text" id="txtLongitude" name="longitude" placeholder="Longitude" class="form-control auto" />
                                        </div>
                                    </div>

                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtAddressCity">City</label>
                                    <div class="col-sm-7">
                                        <input type="text" id="txtAddressCity" name="locality" placeholder="Locality" class="form-control auto" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label">Country *</label>
                                    <div class="col-sm-7">
                                        <select id="ddlCountry2" class="select2-me bfh-countries auto" data-rule-required="true" name="country" style="width: 100%;" data-country="US">
                                        </select>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-3 control-label" for="txtState">State</label>
                                    <div class="col-sm-7">
                                        <select id="ddlState" name="administrative_area_level_1" class="select2-me bfh-states auto" style="width: 100%;" data-country="ddlCountry2">
                                        </select>
                                    </div>
                                </div>
                                <div class="form-group" style="width: 435px !important">
                                    <div id="divMap">
                                        <div id="map-canvas" class="col-sm-6"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <div class="col-sm-offset-2 pull-right" style="margin-top: 2%">
                                        <button type="submit" id="btnSave" class="btn btn-primary">Save</button>
                                        <button type="submit" id="btnSaveNotify" class="btn btn-primary">Save and Notify</button>
                                        <button class="btn btn-info" id="btnClearAll" type="reset" id="btnClear">Clear All</button>
                                        <button class="btn btn-danger" type="button" data-dismiss="modal" aria-hidden="true" id="btnCancel">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>


    <%-- CREATE NEW CUSTOMER --%>
    <div id="mdlCustomer" class="modal primary fade" aria-hidden="true" aria-labelledby="myModalLabel" role="dialog" tabindex="-1">
        <div class="modal-dialog" style="width: 50%;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" id="Button1" aria-hidden="true">&times;</button>
                    <h3 class="modal-title"><strong id="Strong1">Create Customer</strong></h3>
                </div>
                <div class="modal-body">
                    <form class="form-horizontal form-validate" id="frmCustomer">
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtFirstName">
                                First Name *</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtFirstName" name="FirstName" data-rule-required="true" class="form-control"
                                    placeholder="First Name" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtLastName">
                                Last Name *</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtLastName" name="LastName" data-rule-required="true" class="form-control"
                                    placeholder="Last Name" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtUsername">
                                Display Name *</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtUsername" name="txtUsername" data-rule-required="true"
                                    class="form-control" placeholder="Display Name" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtCompany">
                                Company *</label>
                            <div class="col-sm-7">
                                <input type="text" name="company" data-rule-required="true" id="txtCompany" class="form-control"
                                    placeholder="Company" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtEmail">
                                Email*</label>
                            <div class="col-sm-7">
                                <input type="text" name="email" data-rule-required="true" data-rule-email="true" id="txtEmail" class="form-control"
                                    placeholder="Company Email" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtDateOfBirth">
                                Date Of Birth</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtDateOfBirth" name="dateofbirth" readonly class="form-control input-datepicker"
                                    data-date-format="mm/dd/yyyy" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtCustDescription">
                                Description</label>
                            <div class="col-sm-7">
                                <textarea name="txtCustDescription" id="txtCustDescription" class="form-control"
                                    placeholder="Description.."></textarea>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                Sex *</label>
                            <div class="col-md-9">
                                <label class="radio-inline" for="male">
                                    <input type="radio" id="male" name="Sex" checked value="True">
                                    Male
                                </label>
                                <label class="radio-inline" for="female">
                                    <input type="radio" id="female" name="Sex" value="False">
                                    Female
                                </label>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtPhoneNumber">
                                Phone Number *</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtPhoneNumber" name="phonenumber" data-rule-required="true"
                                    placeholder="Phone Number" class="form-control" onkeypress="return isNumberKey(event)" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtMobileNumber">
                                Mobile Number</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtMobileNumber" name="mobilenumber" placeholder="Mobile Number"
                                    class="form-control" onkeypress="return isNumberKey(event)" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtCustAddress">
                                Address</label>
                            <div class="col-sm-7">
                                <textarea class="form-control" id="txtCustAddress" name="txtCustAddress" placeholder="Address"></textarea>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtCity">
                                City</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtCity" name="city" placeholder="City" class="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label" for="txtZipCode">
                                ZipCode</label>
                            <div class="col-sm-7">
                                <input type="text" id="txtZipCode" name="txtZipCode" placeholder="Zip Code" class="form-control"
                                    onkeypress="return isNumberKey(event)" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                Country</label>
                            <div class="col-sm-7">
                                <select id="ddlCountry" class="select2-me bfh-countries" style="width: 100%;" data-country="US">
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label">
                                Image</label>
                            <div class="col-sm-7">
                                <input type="hidden" id="hdnimgFile" />
                                <div class="fileupload-preview thumbnail" id="imagediv" style="width: 110px; height: 130px;">
                                </div>
                                <div id="mulitplefileuploader">
                                    Upload
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <div class="form-group">
                                <div class="col-sm-offset-2 pull-right" style="margin-top: 2%">
                                    <button type="submit" id="Button2" class="btn btn-primary">
                                        Submit</button>
                                    <button class="btn btn-danger" type="button" id="btnCancelCustomer" data-dismiss="modal" aria-hidden="true">
                                        Cancel</button>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>
    <%-- THIS IS THE JS FILE FOR PROJECT WHICH CONTAIN ALL THE JQUERY FUNCTIONS --%>
    <script src="ExtJS/ProjectMaster.js"></script>
</asp:Content>
