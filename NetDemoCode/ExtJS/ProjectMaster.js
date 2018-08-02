var oTable = "", oTable2 = "", Globalrow;
var ProjectLink = 0;

var sources = {
    C: "CLOSED",
    E: "ESTIMITING",
    I: "IN PROGRESS",
    N: "NEW",
    T: "TO MEASURE",
    W: "WAITING"
};

$(document).ready(function (e) {

    $("#startdate").val("");
    $("#enddate").val("");
    $('#ddlProjectStatus').select2("val", "");
    LoadTable();
    GetProjectNo();
    LoadCustomer();


    $('#txtLatitude,#txtLongitude').blur(function (e) {
        debugger;
        FillMap();
    })

    // VALIDATE FORM
    $('.form-validate').each(function () {
        var id = $(this).attr('id');
        debugger;

        $("#" + id).validate({
            errorElement: 'div',
            errorClass: 'help-block animation-slideUp has-error',
            errorPlacement: function (error, element) {
                if (element.parents("label").length > 0) {
                    element.parents("label").after(error);
                } else {
                    element.after(error);
                }
            },
            highlight: function (label) {
                $(label).closest('.form-group').removeClass('has-error has-success').addClass('has-error');
            },
            success: function (label) {
                label.addClass('valid').closest('.form-group').removeClass('has-error has-success').addClass('has-success');

            },
            onkeyup: function (element) {
                $(element).valid();
            },
            onfocusout: function (element) {
                $(element).valid();
            },
            submitHandler: function () {
                debugger;
                if (id == "frmCustomer") {
                    SaveCustomer();
                }
                else if (id == "frmProjectMaster") {
                    var x = $(this)[0].submitButton.id;
                    if (x == "btnSaveNotify") {
                        Save(1);
                    }
                    else {
                        Save(0);
                    }
                }

            }
        })
    });

    $('#btnClear').click(function (e) { 
        setTimeout(function (e) {
            $('#hdnLink').val('');
            $('#hdnProjectNo').val("");
            FillDate();
            GetProjectNo();
        }, 500);
    })

    $('#Btnfilterclear').click(function (e) {
        LoadTable();
        GetProjectNo();
        LoadCustomer();

        $("#startdate").val("");
        $("#enddate").val("");
        $("#ddlcustomername").select2("val", 0);
        $("#ddlusername").select2("val", 0);
        $("#ddlstatus").select2("val", 0);
    })

    $('#btnNewProject').click(function (e) {
        $('#ddlCustomer').select2("val", 0);
        $('#ddlUser').select2("val", 0);
        $('#ddlProjectStatus').select2("val", 0);
        $('#ddlUser2').select2("val", 0);
        flushdata();
        GetProjectNo();
        $('#divMap')[0].innerHTML = '<div id="map-canvas" class="col-sm-6"></div>';
        $('#btnClearAll').show();
        //  show_modal('<iframe onload="iframeLoaded()" class="IframeEdit"  src="iframeProjectMaster.aspx" width="100%" height="100%" style="border:0"></iframe>', "Create New Project");
    })
    // FILE UPLOAD USING HANDLER
    var settings = {
        url: 'Handlers/FileUploader.ashx',
        method: "POST",
        allowedTypes: "jpg,JPG,jpeg,gif,png",
        fileName: "myfile",
        multiple: true,
        onSuccess: function (files, data, xhr) { 
            $('.ajax-file-upload-statusbar').hide();
            $('#hdnimgFile').val(data);
            var x = "<img src=savedimages/" + data + " style='width: 100px;height: 120px;'></img>"
            var y = $('#imagediv');
            $('#imagediv')[0].innerHTML = x;
        },
        onError: function (files, status, errMsg) {
            $("#status").html("<font color='red'>Upload is Failed</font>");
        }
    }

    $("#mulitplefileuploader").uploadFile(settings);

    $('#btnNewCustomer').click(function (e) {
        $('#mdlProject').modal('hide');
        setTimeout(function (e) {
            $('#mdlCustomer').modal('show');
        }, 500)
    })

    $('#btnCancelCustomer').click(function (e) {
        setTimeout(function (e) {
            $('#mdlProject').modal('show');
        }, 500);
    })

    $("#chkClosed").change(function () { 
        $('#Btnfilter').click();
    });

});

// SAVE CUSTOMER USING WEBSERVICE
function SaveCustomer() {

    var ob = {};
    ob["Link"] = "0";
    ob["CustomerNo"] = "0";
    ob["FirstName"] = $('#txtFirstName').val();
    ob["LastName"] = $('#txtLastName').val();
    ob["Username"] = $('#txtUsername').val();
    ob["Company"] = $('#txtCompany').val();
    ob["Description"] = $('#txtCustDescription').val();
    ob["Email"] = $('#txtEmail').val();
    ob["DateOfBirth"] = $('#txtDateOfBirth').val();
    ob["OfficeNumber"] = $('#txtPhoneNumber').val();
    ob["MobileNumber"] = $('#txtMobileNumber').val();
    ob["Sex"] = $('input[name=Sex]:checked').val();
    ob["Address"] = $('#txtCustAddress').val();
    ob["ZipCode"] = $('#txtZipCode').val();
    ob["City"] = $('#txtCity').val();
    ob["Country"] = $('#ddlCountry').val();
    ob["Image"] = $('#hdnimgFile').val();

    var datalist = [];

    var Data = {
        CustomerData: ob,
        ContactData: datalist
    }

    var savedata = RequestAJAX("WebService/WebService.asmx/SaveCustomer", JSON.stringify(Data));
    if (savedata.d.indexOf("Successfully") != -1) {
        ShowMessage("success", "Success !", savedata.d);
        LoadCustomer();
        $('.form-validate').trigger("reset");
        $('#hdnimgFile').val("");
        $('#mdlCustomer').modal('hide');
        $('#mdlProject').modal('show');
    }
    else {
        ShowMessage("danger", "Warning !", savedata.d);
    }

}

// CHANGE STATUS OF THE PROJECT.
function Editable() {

    $('.status').editable({
        prepend: "Select Status",
        source: [
            { value: "C", text: 'CLOSED' },
            { value: "E", text: 'ESTIMITING' },
            { value: "I", text: 'IN PROGRESS' },
            { value: "N", text: 'NEW' },
            { value: "T", text: 'TO MEASURE' },
            { value: "W", text: 'WAITING' },
            
        ],
        success: function (response, sourceData, newValue) {
            debugger;
            var x = response;
            var Data = {
                ProjectLink: ProjectLink,
                Flag: sourceData
            }

            $.ajax({
                url: "WebService/WebService.asmx/SaveProjectStatus",
                data: JSON.stringify(Data),
                type: 'POST',
                contentType: 'application/json;',
                dataType: 'json',
                success: function (result) {
                    $('#Btnfilter').click();
                    ProjectLink = 0;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {

                    alert(XMLHttpRequest.responseText);
                    alert(textStatus);
                    alert(errorThrown);
                }
            });


        }
     
    });
}

// LOAD DATA INTO THE JQUERY DATATABLE
function LoadTable() {
     
    var str = "";
    if ($("#chkClosed").prop("checked") == true) {
        str = str + "and projectmaster.Flag = 'C'";
    }
    else if ($("#chkClosed").prop("checked") == false) {
        str = str + "and projectmaster.Flag != 'C'";
    }


    var Data = {
        filter: str
    }


    $.ajax({
        url: "WebService/WebService.asmx/GetAllProjectFilter",
        data: JSON.stringify(Data),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != null) {
                debugger;
                var userlist = result;
                var t = JSON.parse(userlist.d);

                $('#ProjectList').remove();
                $('#ProjectList_wrapper').remove();
                var container = document.getElementById('tblContainer');
                container.innerHTML = '<table class="table table-hover table-nomargin dataTable table-bordered" id="ProjectList"></table>';
                oTable = $('#ProjectList').dataTable({
                    "bRetrieve": false,
                    "bSortClasses": false,
                    "bLengthChange": true,
                    "bPaginate": true,
                    "bAutoWidth": true,
                    "aaSorting": [1, "Desc"],
                    "aaData": t,
                    "drawCallback": function (settings) {
                        $('[data-toggle="tooltip"], .enable-tooltip').tooltip({ container: "body", animation: !1 });
                        Editable();
                    },
                    "aoColumns": [
                    {
                        "sTitle": "",
                        "mData": null,
                        "class": "details-control",
                        "bSortable": false,
                        "render": function (obj) {
                            return '';
                        }
                    },
                    { "sTitle": "Link", "mData": "Link", "bVisible": false },
                    {
                        "sTitle": "ProjectNo",
                        "mData": null,
                        "render": function (oObj) {
                            return '<a href="ProjectDetail.aspx?ProjectNo=' + oObj.ProjectNo + '" onclick="" ' +
                            ')">' + oObj.ProjectNo + '</a>';
                        }
                    },
                    { "sTitle": "Project Name", "mData": "ProjectName" },
                    { "sTitle": "Customer", "mData": "CustomerName" },
                    { "sTitle": "User Assigned", "mData": "UserName" },
                    {
                        "sTitle": "Date Started",
                        "mData": null,
                        "render": function (obj) {
                            return GetProperDate2(obj.KickOffDate);
                        }
                    },
                    {
                        "sTitle": "Status",
                        "mData": null,
                        "render": function (obj) {
                            return '<a href="#" class="status" data-type="select" data-pk="1" data-value="" data-original-title="Select Status">' + sources[obj.Flag] + '</a>'; Editable();
                        }
                    },
                    { "sTitle": "Project Location", "mData": "Address" },
                    {
                        "sWidth": "100px",
                        "sTitle": "Actions",
                        "mData": null,
                        "bSortable": false,
                        "render": function (oObj) {
                            return '<div class="text-center"><a href="ProjectDetail.aspx?ProjectNo=' + oObj.ProjectNo + '" onclick="" data-toggle="tooltip" data-original-title="Open"  class="btn btn-effect-ripple btn-sm btn-info" ' +
                               ')"><i class="fa fa-folder-open"></i></a><a href="iframeProjectMaster.aspx?ProjectNo=' + oObj.ProjectNo + '" data-toggle="tooltip" data-original-title="Edit"  class="fancybox fancybox.iframe btn btn-effect-ripple btn-sm btn-success" ' +
                               ')"><i class="fa fa-pencil"></i></a><button type="button" data-toggle="tooltip" data-original-title="Duplicate"  class="btn btn-effect-ripple btn-sm btn-warning" ' +
                               ')"><i class="fa fa-files-o"></i></button></div>';
                        }
                    }
                    ]
                });

                $(".dataTables_filter input").attr("placeholder", "Search");
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            alert(XMLHttpRequest.responseText);
            alert(textStatus);
            alert(errorThrown);
        }
    });

}

/* Nested Table  */

// DISPLAY DETAILS INTO THE NESTED TABLE ON CLICK + ICON FOR SPECIFIC COLUMN

$(document).on('click', '#ProjectList tbody tr td.details-control', function () {
     
    var $tr = $(this).parents('tr');
    var aData = oTable.fnGetData($tr[0]._DT_RowIndex);

    var Data = {
        ProjectNo: aData["ProjectNo"]
    }

    $.ajax({
        url: "WebService/WebService.asmx/GetItemByProjectNo",
        data: JSON.stringify(Data),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != null) {
                debugger;
                var userlist = result;
                var t = JSON.parse(userlist.d);

                $('#ItemList').remove();
                $('#ItemList_wrapper').remove();
                var container = document.getElementById('tblItemList');
                container.innerHTML = '<table style="width: 98%;padding-left: 3%;" class="table table-hover table-nomargin dataTable table-bordered" id="ItemList"></table>';
                $('#ItemList').dataTable({
                    "bRetrieve": false,
                    "bSortClasses": false,
                    "bLengthChange": true,
                    "bPaginate": false,
                    "bAutoWidth": true,
                    "aaSorting": [],
                    "bFilter": false,
                    "aaData": t,
                    "aoColumns": [
                    { "sTitle": "ItemNo", "mData": "ItemNo" },
                    {
                        "sTitle": "ItemName",
                        "mData": null,
                        "render": function (obj) {
                            if (obj.PackageNo != "" && obj.PackageNo != null) {
                                return obj.PackageName;
                            }
                            else
                                return obj.ItemName;
                        }
                    },
                    { "sTitle": "Quantity", "mData": "Qty" },
                    { "sTitle": "Height", "mData": "Height" },
                    { "sTitle": "Width", "mData": "Width" },
                    { "sTitle": "Fraction Height", "mData": "FractionHeight" },
                    { "sTitle": "Fraction Width", "mData": "FractionWidth" },
                    { "sTitle": "Rate($)", "mData": "Rate" },
                    { "sTitle": "Total", "mData": "Total" },
                    ]
                });

                $(".dataTables_filter input").attr("placeholder", "Search");

                debugger;


                if ($tr.hasClass('shown')) {
                    $tr.removeClass('shown');
                    $tr.next('tr.chk').remove();
                }
                else {

                    $("#ProjectList tbody tr").each(function (e) {

                        var $alltr = $(this);
                        if ($alltr.hasClass('shown')) {
                            $alltr.removeClass('shown');
                            $alltr.next('tr.chk').remove();
                        }
                    })

                    $tr.addClass('shown');
                    $tr.after('<tr class="chk"><td  colspan="9"> ' + container.innerHTML + ' </td></tr>');
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            alert(XMLHttpRequest.responseText);
            alert(textStatus);
            alert(errorThrown);
        }
    }); 
 
});

// EDIT AND DELETE BUTTON FOR EDIT AND DELETE.
$(document).on("click", "#ProjectList tbody tr td button", function (e) {

    var ClassName = $(this)[0].firstChild.className;

    if (ClassName == "fa fa-pencil") {
        // WHEN PENCIL ICON CLASS CLICK THEN EDIT PROJECT
        var row = $(this).closest("tr").get(0);
        Globalrow = row._DT_RowIndex;
        EditProject(Globalrow);
    }
    else if (ClassName == "fa fa-files-o") {
        
        var row = $(this).closest("tr").get(0);
        Globalrow = row._DT_RowIndex;
        var aData = oTable.fnGetData(Globalrow);

        var ob = {}
        ob["Link"] = 0;
        ob["Flag"] = aData["Flag"];
        ob["ProjectNo"] = 0;
        ob["CustomerNo"] = aData["CustomerNo"];
        ob["ProjectName"] = aData["ProjectName"];
        ob["Description"] = aData["Description"];
        ob["AssignUser"] = aData["AssignUser"];
        ob["KickOffDate"] = aData["KickOffDate"];
        ob["Address"] = aData["Address"];
        ob["Latitude"] = aData["Latitude"];
        ob["Longitude"] = aData["Longitude"];
        ob["InitialRequirement"] = aData["InitialRequirement"];
        ob["SalesRepresentive"] = aData["SalesRepresentive"];
        ob["Country"] = aData["Country"];
        ob["State"] = aData["State"];
        ob["ContactName1"] = aData["ContactName1"];
        ob["ContactPhone1"] = aData["ContactPhone1"];
        ob["ContactName2"] = aData["ContactName2"];
        ob["ContactPhone2"] = aData["ContactPhone2"];

        var Data = {
            ProjectMaster: ob,
            notify: 0
        }

        var savedata = RequestAJAX("WebService/WebService.asmx/SaveProject", JSON.stringify(Data));
        if (savedata.d.indexOf("Successfully") != -1) {
            debugger
            ShowMessage("success", "Success !", savedata.d);
            $('#hdnLink').val('');
            flushdata();
            GetProjectNo();
            LoadTable();
            $('#mdlProject').modal('hide');
        }
        else {
            ShowMessage("danger", "Warning !", savedata.d);
        }


    }

});

$(document).on("click", "#ProjectList tbody tr td a", function (e) {
    
    var row = $(this).closest("tr").get(0);
    Globalrow = row._DT_RowIndex;
    var aData = oTable.fnGetData(row);
    ProjectLink = aData["Link"];

});


// FILTER AND AGAIN BIND DATA INTO THE DETATABLE
$(document).on("click", "#Btnfilter", function (e) {
    
    var str = "";
    if ($("#ddlcustomername").val() != null) {
        str = str + "AND projectmaster.CustomerNo in  (\"" + $("#ddlcustomername").val().join('","') + "\") ";
    }
    if ($("#ddlusername").val() != null) {
        str = str + "AND projectmaster.AssignUser in (\"" + $("#ddlusername").val().join('","') + "\") ";
    }
    if ($("#ddlstatus").val() != null) {
        str = str + "AND projectmaster.Flag in (\"" + $("#ddlstatus").val().join('","') + "\") ";
    }
    if ($("#startdate").val() != "") {
        var sts = $("#startdate").val().split("/");
        var ysts = sts[2] + "-" + sts[0] + "-" + sts[1];
        str = str + "AND projectmaster.KickOffDate >= '" + ysts + "' ";
    }
    if ($("#enddate").val() != "") {
        var sts = $("#enddate").val().split("/");
        var ysts = sts[2] + "-" + sts[0] + "-" + sts[1];
        str = str + "AND projectmaster.KickOffDate <= '" + ysts + "' ";
    }
    if ($("#chkClosed").prop("checked") == true) {
        str = str + "and projectmaster.Flag = 'C'";
    }
    else if ($("#chkClosed").prop("checked") == false) {
        str = str + "and projectmaster.Flag != 'C'";
    }


    var Data = {
        filter: str
    }

    $.ajax({
        url: "WebService/WebService.asmx/GetAllProjectFilter",
        data: JSON.stringify(Data),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != null) {
                var userlist = result;
                var t = JSON.parse(userlist.d);

                $('#ProjectList').remove();
                $('#ProjectList_wrapper').remove();
                var container = document.getElementById('tblContainer');
                container.innerHTML = '<table class="table table-hover table-nomargin dataTable table-bordered" id="ProjectList"></table>';
                oTable = $('#ProjectList').dataTable({
                    "bRetrieve": false,
                    "bSortClasses": false,
                    "bLengthChange": true,
                    "bPaginate": true,
                    "bAutoWidth": true,
                    "aaSorting": [1, "Desc"],
                    "aaData": t,
                    "drawCallback": function (settings) {
                        $('[data-toggle="tooltip"], .enable-tooltip').tooltip({ container: "body", animation: !1 });
                        Editable();
                    },
                    "aoColumns": [
                    {
                        "sTitle": "",
                        "mData": null,
                        "class": "details-control",
                        "bSortable": false,
                        "render": function (obj) {
                            return '';
                        }
                    },
                    { "sTitle": "Link", "mData": "Link", "bVisible": false },
                    {
                        "sTitle": "ProjectNo",
                        "mData": null,
                        "render": function (oObj) {
                            return '<a href="ProjectDetail.aspx?ProjectNo=' + oObj.ProjectNo + '" onclick="" ' +
                            ')">' + oObj.ProjectNo + '</a>';
                        }
                    },
                    { "sTitle": "Project Name", "mData": "ProjectName" },
                    { "sTitle": "Customer", "mData": "CustomerName" },
                    { "sTitle": "User Assigned", "mData": "UserName" },
                    {
                        "sTitle": "Date Started",
                        "mData": null,
                        "render": function (obj) {
                            return GetProperDate2(obj.KickOffDate);
                        }
                    },
                    {
                        "sTitle": "Status",
                        "mData": null,
                        "render": function (obj) {
                            return '<a href="#" class="status" data-type="select" data-pk="1" data-value="" data-original-title="Select Status">' + sources[obj.Flag] + '</a>'; Editable();
                        }
                    },
                    { "sTitle": "Project Location", "mData": "Address" },
                    {
                        "sWidth": "100px",
                        "sTitle": "Actions",
                        "mData": null,
                        "bSortable": false,
                        "render": function (oObj) {
                            return '<div class="text-center"><a href="ProjectDetail.aspx?ProjectNo=' + oObj.ProjectNo + '" onclick="" data-toggle="tooltip" data-original-title="Open"  class="btn btn-effect-ripple btn-sm btn-info" ' +
                               ')"><i class="fa fa-folder-open"></i></a><a href="iframeProjectMaster.aspx?ProjectNo=' + oObj.ProjectNo + '" data-toggle="tooltip" data-original-title="Edit"  class="fancybox fancybox.iframe btn btn-effect-ripple btn-sm btn-success" ' +
                               ')"><i class="fa fa-pencil"></i></a><button type="button" data-toggle="tooltip" data-original-title="Duplicate"  class="btn btn-effect-ripple btn-sm btn-warning" ' +
                               ')"><i class="fa fa-files-o"></i></button></div>';
                        }
                    }
                    ]
                });

                $(".dataTables_filter input").attr("placeholder", "Search");
            }
        }
    });


});

// EDIT , FIRST FILL INFORMATION IN APPROPRIATE FIELD AND THEN UPDATE IT.
function EditProject(row) { 
    $('#btnClearAll').hide();
    var aData = oTable.fnGetData(row);
    $('#hdnLink').val(aData["Link"]);
    $('#hdnStatus').val(aData["Flag"]);
    $('#ddlProjectStatus').select2("val", aData["Flag"]);
    $('#txtProjectId').val(aData["ProjectNo"]);
    $("#txtprojectEmail").val(aData["ProjectEmail"]);
    $("#txtProjectName").val(aData["ProjectName"]);
    $("#txtDescription").val(aData["Description"]);
    $("#txtDate").val(GetProperDate2(aData["KickOffDate"]));
    $("#txtAddress").val(aData["Address"]);
    $("#txtLatitude").val(aData["Latitude"]);
    $("#txtLongitude").val(aData["Longitude"]);
    $("#ddlCustomer").select2("val", aData["CustomerNo"]);
    $("#ddlUser").select2("val", aData["AssignUser"]);
    $('#txtDate').datepicker('update');
    $('#txtInitialReq').val(aData["InitialRequirement"]);
    $('#ddlUser2').select2("val", aData["SalesRepresentive"]);
    $('#ddlCountry2').select2("val", aData["Country"]);
    $("#txtCon1Name").val(aData["ContactName1"]);
    $("#txtCon1Phone").val(aData["ContactPhone1"]);
    $("#txtCon2Name").val(aData["ContactName2"]);
    $("#txtCon2Phone").val(aData["ContactPhone2"]);
    $('#txtUnitNo').val(aData["UnitNo"]);
    $('#txtAddressCity').val(aData["City"]);

    var country = aData["Country"]
    var State = $('#ddlState');
    $('#ddlState').html('');
    $('#ddlState').select2('val', '');
    for (state in BFHStatesList[country]) {
        if (BFHStatesList[country].hasOwnProperty(state)) {
            State.append('<option value="' + BFHStatesList[country][state].code + '">' + BFHStatesList[country][state].name + '</option>');
        }
    }
    $('#ddlState').select2("val", aData["State"]);



    FillMap();
    setTimeout(function (e) {
        $("#txtLongitude").trigger("blur");
    }, 200)

    $('#mdlProject').modal("show");
}

function GetParameterValues(param) {
    var url = location.href.split('=');
    return url;
}

// GET NEXT PROJECT NUMBER
function GetProjectNo() {

    var Data = {

        DocType: "PR"
    }

    $.ajax({
        url: "WebService/WebService.asmx/GetNextNo",
        data: JSON.stringify(Data),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != null) {
                var t = JSON.parse(result.d);
                $('#txtProjectId').val(t);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            alert(XMLHttpRequest.responseText);
            alert(textStatus);
            alert(errorThrown);
        }
    });
}

// FILL GOOGLE MAP USING LATITUDE AND LONGITUDE . LATITUDE AND LONGITUDE FIND USING A GOOGLE AUTO ADDRESS FILE JQUERY API
function FillMap() {

    var latitude = $('#txtLatitude').val() == "" ? "0.00" : $('#txtLatitude').val();
    var longitude = $('#txtLongitude').val() == "" ? "0.00" : $('#txtLongitude').val();
    initialize(latitude, longitude);
}

// GET ALL CUSTOMER AND FILL DROPDOWN
function LoadCustomer() { 
    $.ajax({
        url: "WebService/WebService.asmx/GetAllCustomer",
        data: "",
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != null) {
                var userlist = result;
                var t = JSON.parse(userlist.d);

                var ddl = document.getElementById('ddlCustomer');
                $('#ddlCustomer').html('');
                for (var i = 0; i < t.length; i++) {
                    ddl.options[i] = new Option(t[i].UserName, t[i].CustomerNo);
                }
                $('#ddlCustomer').select2("val", "");

                var ddl = document.getElementById('ddlcustomername');
                $('#ddlcustomername').html('');
                for (var i = 0; i < t.length; i++) {
                    ddl.options[i] = new Option(t[i].UserName, t[i].CustomerNo);
                }

                //ddl.options[ddl.options.length] = new Option("-Select-", "0");
                //$('#ddlcustomername').select2("val", 0);

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            alert(XMLHttpRequest.responseText);
            alert(textStatus);
            alert(errorThrown);
        }
    });

} 

// SAVE PROJECT
function Save(notify) {

    if (notify == 1) {
        var Message = "";
        var Flag = false;
        if ($('#txtInitialReq').val() == "") {
            Message = "Please Enter Initial Requirement.<br/>";
            Flag = true;
        }
        if ($('#ddlUser2').val() == "0" || $('#ddlUser2').val() == null) {
            Message += "Please Select Sales Representive.";
            Flag = true

        }

        if (Flag) {
            ShowMessage("danger", "Warning !", Message);
            return false;
        }
    } 

    // CREATE OBJECT TO SAVE IN DATABASE
    var ob = {}
    ob["Link"] = $('#hdnLink').val() == "" ? "0" : $('#hdnLink').val();
    ob["Flag"] = $('#ddlProjectStatus').val();
    ob["ProjectNo"] = $('#txtProjectId').val() == "" ? "0" : $('#txtProjectId').val();
    ob["CustomerNo"] = $("#ddlCustomer").val();
    ob["ProjectEmail"] = $("#txtprojectEmail").val();
    ob["ProjectName"] = $("#txtProjectName").val();
    ob["Description"] = $("#txtDescription").val();
    ob["AssignUser"] = $("#ddlUser").val();
    ob["KickOffDate"] = $("#txtDate").val();
    ob["Address"] = $("#txtAddress").val();
    ob["Latitude"] = $("#txtLatitude").val();
    ob["Longitude"] = $("#txtLongitude").val();
    ob["InitialRequirement"] = $('#txtInitialReq').val();
    ob["SalesRepresentive"] = $('#ddlUser2').val();
    ob["Country"] = $('#ddlCountry2').val();
    ob["State"] = $('#ddlState').val();
    ob["ContactName1"] = $("#txtCon1Name").val();
    ob["ContactPhone1"] = $("#txtCon1Phone").val();
    ob["ContactName2"] = $("#txtCon2Name").val();
    ob["ContactPhone2"] = $("#txtCon2Phone").val();
    ob["UnitNo"] = $('#txtUnitNo').val();
    ob["City"] = $('#txtAddressCity').val();

    var Data = {
        ProjectMaster: ob,
        notify: notify
    } 
    var savedata = RequestAJAX("WebService/WebService.asmx/SaveProject", JSON.stringify(Data));
    if (savedata.d.indexOf("Successfully") != -1) {
        debugger
        ShowMessage("success", "Success !", savedata.d);
        $('#hdnLink').val('');
        flushdata();
        GetProjectNo();
        LoadTable();
        $('#mdlProject').modal('hide');
    }
    else {
        ShowMessage("danger", "Warning !", savedata.d);
    }

}

function ParentMathods(message) {
    LoadTable();
    $.fancybox.close();
}

function initialize(a, b) {
    debugger;
    geocoder = new google.maps.Geocoder();
    var a = a;
    var b = b;
    var latlng = new google.maps.LatLng(a, b);
    var myOptions = {
        zoom: 17,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map-canvas"), myOptions);
    addMarker(a, b);
}

function addMarker(a, b) {

    var myLatlng = new google.maps.LatLng(a, b);

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map
    });


    global_markers = marker;

    google.maps.event.addListener(global_markers, 'click', function () {
        infowindow.setContent(this['infowindow']);
        infowindow.open(map, this);
    });
}

/* Capital */
$(document).on('blur', '.Capitalize', function (e) {
    debugger;

    val = $(this).val();
    newVal = '';
    val = val.split(' ');
    for (var c = 0; c < val.length; c++) {
        newVal += val[c].substring(0, 1).toUpperCase() +
val[c].substring(1, val[c].length) + ' ';
    }
    $(this).val(newVal);

})
 
function ParentNotification() {
    $.fancybox.close();
}



