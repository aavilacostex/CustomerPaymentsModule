<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="CustPaymentModule.aspx.vb" Inherits="CustomerPayments.CustPaymentModule" EnableEventValidation="false" ViewStateMode="Disabled" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">   

    <asp:UpdatePanel ID="updatepnl" runat="server">   
         <Triggers>
             <asp:AsyncPostBackTrigger ControlID="btnPayments1" />
             <asp:PostBackTrigger ControlID="btnGenerateExcel" />
             <%--<asp:AsyncPostBackTrigger ControlID="txtDate" />   --%>
        </Triggers>

        <ContentTemplate>

            <div class="container-fluid">
                <div class="breadcrumb-area breadcrumb-bg">
                    <div class="row">
                        <div class="col-md-offset-4 col-md-8 center">
                            <div class="breadcrumb-inner">
                                <div class="row">
                                    <div class="col-md-11">
                                        <div class="bread-crumb-inner">
                                            <div class="breadcrumb-area page-list">
                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-7 link">
                                                        <i class="fa fa-map-marker"></i>
                                                        <a href="/Default">Home</a>
                                                        " - "
                                                    <span>CUSTOMER PAYMENTS</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="loadOptions" class="col-md-4">
                            <div class="row">
                                <div class="col-md-3" style="display:none">
                                    <asp:LinkButton ID="btnNewItem" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-plus fa-1x"" aria-hidden="true"> </i> NEW ITEM
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3">
                                    <asp:LinkButton ID="btnGenerateExcel" OnClick="btnExcel_Click" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-file-excel-o fa-1x" aria-hidden="true"> </i> GENERATE EXCEL
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3">
                                    <asp:LinkButton ID="btnGeneratePdf" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-list-alt fa-1x" aria-hidden="true"> </i> GENERATE PDF
                                                        </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div> 

            <div id="showActionsSection" class="container-fluid" style="display: none !important;" runat="server">          
                <div id="rowFilters" class="row" runat="server">
                    <div class="col-md-2"></div>
                    <div class="col-md-3">
                        <div class="accordion-wrapper">
                            <div id="accordion_2">
                                <div class="card">
                                    <!--ACCORDION DEFAULT VALUES HEADER-->
                                    <div class="card-header" id="headingOne_2">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_2" aria-expanded="false" aria-controls="collapseOne_2">
                                                <span class="">DEFAULT VALUES <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>

                                    <!--ACCORDION: CONTENT-->
                                    <div id="collapseOne_2" class="collapse show" aria-labelledby="headingOne_2" data-parent="#accordion_2" style="">
                                        <div id="card-body-custom" class="card-body">
                                            <ul class="checklist">
                                                <%--<li><i class="fa fa-check"></i><span id="spnCountItems">COUNT ITEMS:</span><asp:Label ID="lblItemsCount" runat="server"></asp:Label></li>
                                                <li><i class="fa fa-check"></i><span id="spnTimesQuotes">CUSTOMER NUMBER: </span><asp:Label ID="lblCustomerNo" runat="server"></asp:Label> </li>--%>
                                            </ul>
                                        </div>
                                    </div>
                                    <!-- COLLAPSE CONTENT END -->
                                </div>
                                <!-- CARD END -->
                            </div>
                            <!-- ACCORDION END -->
                        </div>
                    </div>
                    <%--<div id="col1-custom" class="col-md-1"></div>--%>
                    <div class="col-md-3">                        
                        <div class="accordion-wrapper">
                            <div id="accordion_3">
                                <div class="card">
                                    <div class="card-header" id="headingOne_3">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_3" aria-expanded="false" aria-controls="collapseOne_3">
                                                <span class="">FILTERS  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>
                                    <div id="collapseOne_3" class="collapse show" aria-labelledby="headingOne_3" data-parent="#accordion_3" style="">
                                        <div class="card-body">
                                            <div id="rowRadios1" class="form-group col-md-12">
                                                <div class="row">
                                                    <div class="form-group col-md-6 radio-toolbar">
                                                        <label class="form-check">
                                                            <p>Customer No.</p>
                                                            <asp:RadioButton ID="rdCustNo" onclick="yesnoCheck('rowCustNo');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                            <span class="checkmark"></span>
                                                        </label>
                                                    </div>

                                                    <div class="form-group col-md-6 radio-toolbar">
                                                        <label class="form-check">
                                                            <p>Start Date</p>
                                                            <asp:RadioButton ID="rdFrom" onclick="javascript:yesnoCheck('rowFrom');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                            <span class="checkmark"></span>
                                                        </label>
                                                    </div>
                                                </div> 
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="accordion-wrapper">
                            <div id="accordion_4">
                                <div class="card">
                                    <div class="card-header" id="headingOne_4">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_4" aria-expanded="false" aria-controls="collapseOne_4">
                                                <span class="">FILTER CRITERIA  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>
                                    <div id="collapseOne_4" class="collapse show" aria-labelledby="headingOne_4" data-parent="#accordion_4" style="">
                                        <div class="card-body">
                                            <!--search by Category-->
                                            <%--<div id="rowCustNo" class="rowCustNo" style="display: none;">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-10" style="max-width:100% !important;">
                                                    
                                                    <div class="form-row">
                                                        <div class="col-md-10">
                                                            <div class="form-group">
                                                                
                                                                <div class="input-group">
                                                                    <asp:Label ID="lblVendor" CssClass="label-style" Text="Vendor" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                                    <br />
                                                                    <asp:TextBox ID="txtvendor" CssClass="form-control fullTextBox" runat="server" />
                                                                    <div class="input-group-append">
                                                                        <asp:LinkButton ID="lnkSearchVendorNo" OnClick="lnkSearchVendorNo_Click" runat="server">
                                                                            <span id="Span100" aria-hidden="true" runat="server">
                                                                                <i class="fa fa-search center-vert font-awesome-custom" aria-hidden="true"></i> 
                                                                            </span>                                                                            
                                                                        </asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>  
                                                        <%--<div class="col-md-7">
                                                            <div class="form-group">
                                                                <asp:Label ID="lblVndDesc" CssClass="label-style" Text="Vendor Description" runat="server"></asp:Label>
                                                                <asp:TextBox ID="txtVndDesc" CssClass="form-control fullTextBox autosuggestvendor" runat="server" />
                                                            </div>                                                            
                                                        </div>--%>
                                                   <%-- </div>
                                                    <div id="rwVndDesc" class="row">
                                                        <div class="col-md-12">
                                                            <asp:Label ID="lblVndDesc" Text="test name for the current selected vendor" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                    
                                                    <%--<asp:DropDownList ID="ddlStatus" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Category." runat="server"></asp:DropDownList>--%>
                                                <%--</div>
                                            </div>--%>
                                            <!--search by VendorName-->
                                            <%--<div id="rowFrom" class="rowVndName" style="display: none;">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-10">
                                                    <label for="sel-vndassigned">Start Date</label>
                                                    <div class="input-group-append">  
                                                        <asp:TextBox ID="txtDate" CssClass="form-control" OnTextChanged="txtDate_TextChanged" name="txt-date" placeholder="MM/DD/AAAA" runat="server" />
                                                        <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                                    </div>                                                    
                                                </div>
                                            </div>--%>                                    
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>  
                </div>
            </div>     
            
            <div id="filters" class="container-fluid"  runat="server">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-8">
                        <div class="row">
                            <%--<div class="col-md-2"></div>
                            <div class="col-md-2" style="display:none !important;">
                                <asp:Panel ID="pnDefValues" CssClass="pnFilterStyles" GroupingText="Response Values" runat="server">
                                    <div id="rowDefValues" class="rowDefValues">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <span id="spnCountItems">COUNT ITEMS:</span>
                                                <asp:Label ID="lblCountItems" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row" style="display: none;">
                                            <div class="col-md-12">
                                                <span id="spnTimesQuotes">CUSTOMER NUMBER: </span>
                                                <asp:Label ID="Label2" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>--%>
                            <div class="col-md-7">
                                <asp:Panel ID="pnRequiredFilters" CssClass="pnFilterStyles" GroupingText="Required Filters" runat="server">
                                    <div id="rowCustNo" class="rowCustNo">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="form-row">
                                                    <div class="col-md-6">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <asp:Label ID="lblVendor" CssClass="label-style" Text="Client Number" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                                <br />
                                                                <asp:TextBox ID="txtvendor" CssClass="form-control fullTextBox" runat="server" />
                                                                <div class="input-group-append">
                                                                    <asp:LinkButton ID="lnkSearchVendorNo" OnClick="lnkSearchVendorNo_Click" runat="server">
                                                                        <span id="Span1" aria-hidden="true" runat="server">
                                                                            <i class="fa fa-search center-vert font-awesome-custom" aria-hidden="true"></i>
                                                                        </span>
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-6" style="padding-top: 5px!important;">
                                                        <div class="row" style="color: #F7F7FD !important;">f</div>
                                                        <div class="row">
                                                            <asp:Label ID="lblVndDesc" Text="" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="rwVndDesc" class="row" style="display: none;">
                                                </div>

                                                <%--<asp:DropDownList ID="ddlStatus" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Category." runat="server"></asp:DropDownList>--%>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="rowFrom" class="rowVndName">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-row">
                                                    <div class="col-md-12">
                                                        <asp:Label ID="Label3" CssClass="label-style" Text="Start Date" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                        <div class="input-group-append">
                                                            <asp:TextBox ID="txtDate" CssClass="form-control" name="txt-date" placeholder="MM/DD/YYYY" runat="server" />
                                                            <span class="input-group-addon"><i class="fa fa-calendar center-vert font-awesome-custom"></i></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-row">
                                                    <div class="col-md-12">
                                                        <asp:Label ID="Label4" CssClass="label-style" Text="End Date" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                        <div class="input-group-append">
                                                            <asp:TextBox ID="txtDateTo" CssClass="form-control" name="txt-dateto" placeholder="MM/DD/YYYY" runat="server" />
                                                            <span class="input-group-addon"><i class="fa fa-calendar center-vert font-awesome-custom"></i></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="rowDesc">
                                        <div class="row" style="margin-top: 1rem;">
                                            <div class="col-md-12">
                                                <div class="form-row">
                                                    <div class="col-md-12">
                                                         <asp:Label ID="lblSearchPayments" Text="Please select a Customer Number and Date in order to get the payments for the selected customer." runat="server"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                            <div class="col-md-5">
                                <div class="row">
                                    <asp:Panel ID="pnOptionalFilters" CssClass="pnFilterStyles" GroupingText="Extra Filters (Optionals)" runat="server">
                                        <div id="rowReceiptNo">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="form-row">
                                                        <div class="col-md-12">
                                                            <asp:Label ID="lblReceiptNo" CssClass="label-style" Text="Receipt Number" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                            <br />
                                                            <div class="input-group-append">
                                                                <asp:TextBox ID="txtReceiptNo" CssClass="form-control fullTextBox" OnTextChanged="txtReceiptNo_TextChanged" name="txt-receipt" runat="server" />
                                                                <span class="input-group-addon"><i class="fa fa-hashtag center-vert font-awesome-custom"></i></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="rowInvoiceNo">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="form-row">
                                                        <div class="col-md-12">
                                                            <asp:Label ID="lblInvoiceNo" CssClass="label-style" Text="Invoice Number" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                            <br />
                                                            <div class="input-group-append">
                                                                <asp:TextBox ID="txtInvoiceNo" CssClass="form-control fullTextBox" OnTextChanged="txtInvoiceNo_TextChanged" name="txt-invoice" runat="server" />
                                                                <span class="input-group-addon"><i class="fa fa-hashtag center-vert font-awesome-custom"></i></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>                               
                                <div id="rowBtnClear" class="form-row" style="float: left !important;">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <div class="input-group-append">
                                                    <asp:LinkButton ID="lnkClear" OnClick="lnkClear_Click" runat="server">
                                                        <span id="Span1341" aria-hidden="true" runat="server">
                                                            Clear Fields
                                                            <%--<i class="fa fa-plus center-vert font-awesome-custom"></i>--%>
                                                        </span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>                               
                            </div>
                            <div class="col-md-2" style="display: none !important;">                                 
                                <div id="rowBtnPay" class="row">
                                    <asp:Button ID="btnPayments1" Text="Get Payments" CssClass="btn btn-primary btnFullSize" OnClick="btnSearchPayments_Click" runat="server" />
                                </div>                        
                            </div>
                        </div>
                    </div>
                    <div class="col-md-2"></div>                    
                </div>
                <div class="row">
                    <div class="col-md-5"></div>
                    <div id="btnNewRow" class="col-md-2">
                        <asp:Button ID="btnSearchPayments" CssClass="btn btn-primary btnFullSize" OnClick="btnSearchPayments_Click" Text="Get Payments" runat="server" />
                    </div>
                    <div class="col-md-5"></div>
                </div>
            </div>
            
            <div class="row" style="display:none">                
                <asp:HiddenField ID="hiddenId2" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId3" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId4" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenName" Value="" runat="server" />

                <asp:HiddenField ID="hdStartDate" Value="" runat="server" />
                <asp:HiddenField ID="hdEndData" Value="" runat="server" />
            </div>

            <div id="rwGetPayments" class="container-fluid">
                <div class="row">                    
                    <div class="col-md-4">
                        <div id="rowPageSize" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText1" Text="Show " runat="server"></asp:Label></div>
                            <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed"><asp:DropDownList name="ddlPageSize" ID="ddlPageSize" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" EnableViewState="true" ViewStateMode="Enabled" class="form-control" runat="server"></asp:DropDownList></div>
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText2" Text=" entries." runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <%--<div class="col-md-2" style="display: none !important;">
                        <asp:Button ID="Button1" CssClass="btn btn-primary btnFullSize" OnClick="btnSearchPayments_Click" Text="Get Payments" runat="server" />
                    </div>--%>
                    <div class="col-md-8">
                    </div>
                </div>                
            </div>

            <div id="gridSection" class="container-fluid" runat="server">
                <div class="panel panel-default">
                    <div class="panel-body">                
                        <div class="form-horizontal"> 
                            <div id="rowGridView">
                                <asp:GridView ID="grvCustPayment" runat="server" AutoGenerateColumns="false"
                                    PageSize="10" CssClass="table table-striped table-bordered" AllowPaging="True" AllowSorting="true"
                                    GridLines="None" OnRowCommand="grvCustPayment_RowCommand" OnPageIndexChanging="grvCustPayment_PageIndexChanging"
                                    OnRowDataBound="grvCustPayment_RowDataBound" OnSorting="grvCustPayment_Sorting" ShowHeader="true" ShowFooter="true" 
                                    OnRowUpdating="grvCustPayment_RowUpdating" DataKeyNames="invoice" >
                                    <Columns>                          
                                        <asp:BoundField DataField="custno" HeaderText="CUSTOMER" ItemStyle-Width="5%" SortExpression="custno" />
                                        <asp:BoundField DataField="receipt" HeaderText="RECEIPT" ItemStyle-Width="5%" SortExpression="receipt" />
                                        <%--<asp:BoundField DataField="invoice" HeaderText="INVOICE" ItemStyle-Width="5%" SortExpression="invoice" />--%>

                                        <asp:TemplateField HeaderText="INVOICE" ItemStyle-Width="5%" SortExpression="invoice" >
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <EditItemTemplate>  
                                                <asp:Label ID="lblInvoiceNoGrd" runat="server" Text='<%# Bind("invoice") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lbInvoiceNo"
                                                    runat="server"
                                                    TabIndex="1" CommandName="ShowInvoice"
                                                    ToolTip="Click Here" CssClass="clickme" CommandArgument='<%#Eval("invoice") %>'>
                                                    <span id="Span3222" aria-hidden="true" runat="server">
                                                         <asp:Label ID="txtInvoiceNo" Text='<%# Bind("invoice") %>' runat="server"></asp:Label>
                                                    </span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        
                                        <%--<asp:BoundField DataField="invoice" HeaderText="INVOICE" ItemStyle-Width="3%" SortExpression="invoice" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />                                        --%>
                                        <%--<asp:BoundField DataField="WHLDATE" HeaderText="DATE" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="3%" />--%>
                                        <asp:TemplateField HeaderText="INVOICE DATE" SortExpression="invdt" >
                                            <ItemTemplate>
                                                <asp:Literal ID="Literal1" runat="server"
                                                    Text='<%#String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(Eval("invdt"))) %>'>        
                                                </asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:BoundField DataField="invamt" HeaderText="INVOICE AMOUNT" ItemStyle-Width="15%" SortExpression="invamt" />
                                        <asp:BoundField DataField="paid" HeaderText="PAID" ItemStyle-Width="3%" SortExpression="paid" />                                      
                                        <asp:TemplateField HeaderText="PAID DATE" SortExpression="paydt" >
                                            <ItemTemplate>
                                                <asp:Literal ID="Literal111" runat="server"
                                                    Text='<%#String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(Eval("paydt"))) %>'>        
                                                </asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:BoundField DataField="Balance" HeaderText="BALANCE" ItemStyle-Width="6%" SortExpression="Balance" />
                                        <%--<asp:BoundField DataField="Balance" HeaderText="BALANCE" ItemStyle-Width="6%" SortExpression="Balance"  ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />--%>
                                        <asp:TemplateField HeaderText="DETAILS">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkExpander" runat="server" TabIndex="1" ToolTip="Get Reference Detail" CssClass="click-in" CommandName="show"
                                                    OnClientClick='<%# String.Format("return divexpandcollapse(this, {0});", Eval("invoice")) %>'>
                                                    <span id="Span11" aria-hidden="true" runat="server">
                                                        <i class="fa fa-folder"></i>
                                                    </span>
                                                </asp:LinkButton>

                                                <%--</td>
                                                    <tr>
                                                        <td colspan="17" class="padding0">
                                                            <div id="div<%# Eval("InvNo") %>" class="divCustomClass">
                                                                <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="false" GridLines="None" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="IMMOD" HeaderText="MODEL" ItemStyle-Width="15%" SortExpression="IMMOD" />
                                                                        <asp:BoundField DataField="IMCATA1" HeaderText="CATEGORY" ItemStyle-Width="10%" SortExpression="IMCATA1" />
                                                                        <asp:BoundField DataField="subcatdesc" HeaderText="SUBCAT" ItemStyle-Width="15%" SortExpression="SUBCAT" />
                                                                        <asp:BoundField DataField="IMPC1" HeaderText="MAJOR" ItemStyle-Width="7%" SortExpression="IMPC1" />
                                                                        <asp:BoundField DataField="minordesc" HeaderText="MINOR" ItemStyle-Width="15%" SortExpression="IMPC2" />
                                                                        <asp:BoundField DataField="a3comment" HeaderText="COMMENT" ItemStyle-Width="25%" SortExpression="IMPC2" />
                                                                    </Columns>
                                                                    <HeaderStyle BackColor="#95B4CA" ForeColor="White" />
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>                                    
                                    <PagerSettings  Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" pagebuttoncount="10"  />
                                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />                            
                                    <FooterStyle CssClass="footer-style" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />    
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>   

    <%--<script src="https://code.jquery.com/jquery-3.2.1.slim.min.js"></script>--%>         
    <%--<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js"></script>--%>
    <%--<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.5/umd/popper.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap4-input-clearer.js"></script>--%>

    <script type="text/javascript"> 

        function messageFormSubmitted(mensaje, show) {
            //debugger
            messages.alert(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }

        function isActivePanel(activePanel, valorActive) {
            debugger
           
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                        
            if (valorActive == 2) {
                if ($('#<%=hiddenId2.ClientID %>').val() == "0") {
                    $('#<%=hiddenId2.ClientID %>').val("1");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId2.ClientID %>').val("0");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }
            if (valorActive == 3) {
                if ($('#<%=hiddenId3.ClientID %>').val() == "0") {
                    $('#<%=hiddenId3.ClientID %>').val("1");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId3.ClientID %>').val("0");
                   <%-- $('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            if (valorActive == 4) {
                if ($('#<%=hiddenId4.ClientID %>').val() == "0") {
                    $('#<%=hiddenId4.ClientID %>').val("1");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId4.ClientID %>').val("0");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            JSFunction();
        }  
        
        function JSFunction() {
            __doPostBack('<%= updatepnl.ClientID  %>', '');
        }

        function afterRadioCheck(hdFieldId, divId) {
            //debugger

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        function afterDdlCheck(hdFieldId, divId) {
            //debugger        

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        function yesnoCheck(id) {
            debugger

            x = document.getElementById(id);
            xstyle = document.getElementById(id).style;

            var divs = ["rowCustNo", "rowFrom"];

            var i;
            for (i = 0; i < divs.length; i++) {
                //text += divs[i] + "<br>";
                if (divs[i] != id) {
                    //x = document.getElementById(divs[i]).style;
                    x = document.getElementById(divs[i]);
                    xstyle = x.style;
                    xstyle.display = "none";
                } else {
                    //x = document.getElementById(divs[i]).style;
                    x = document.getElementById(divs[i]);
                    xstyle = x.style;
                    xstyle.display = "block";
                    $('#<%=hiddenName.ClientID %>').val(id);
                    //x.display = "block";
                }
            }

            var collapse2 = document.getElementById('collapseOne_2');
            var collapse3 = document.getElementById('collapseOne_3');
            var collapse4 = document.getElementById('collapseOne_4');

            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;    
            
            if (hd3 == 1) {              
                $('#<%=hiddenId4.ClientID %>').val("1");                
                hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            }            
            else { hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value; }

            afterRadioCheck(hd2, collapse2)
            afterRadioCheck(hd3, collapse3)
            afterRadioCheck(hd4, collapse4)           
        }

        function yesnoCheckCustom(id) {
            //debugger

            if (id != "") {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowCustNo", "rowFrom"];

                var i;
                for (i = 0; i < divs.length; i++) {
                    //text += divs[i] + "<br>";
                    if (divs[i] != id) {
                        //x = document.getElementById(divs[i]).style;
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "none";
                    } else {
                        //x = document.getElementById(divs[i]).style;
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "block";
                        $('#<%=hiddenName.ClientID %>').val(id);
                        //x.display = "block";
                    }
                }                
            }

        }

        $('body').on('click', '#accordion_4 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse4 = document.getElementById('collapseOne_4');
            isActivePanel(collapse4, 4);
        });

        $('body').on('click', '#accordion_3 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse3 = document.getElementById('collapseOne_3');
            isActivePanel(collapse3, 3);
        });

        $('body').on('click', '#accordion_2 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse2 = document.getElementById('collapseOne_2');
            isActivePanel(collapse2, 2);
        });

        function dateComparer() {
            var strDateStart = document.getElementById('<%=hdStartDate.ClientID%>').value;
            var strDateEnd = document.getElementById('<%=hdEndData.ClientID%>').value;

            var dtStart = new Date(strDateStart);
            var dtEnd = new Date(strDateEnd);

            if (dtStart > dtEnd || dtEnd < dtStart) {
                return false
            }
            else {
                return true;
            }
        }

        $(function () {

            debugger

            $('#MainContent_txtDate').datepicker(
                {
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100',
                    onSelect: function (date) {
                        debugger
                        var date2 = $("#<%= txtDate.ClientID %>").datepicker('getDate');
                            date2.setDate(date2.getFullYear() + 1);
                        $("#<%= txtDateTo.ClientID %>").datepicker('setDate', date2);
                    }
                }); 

            $('#MainContent_txtDateTo').datepicker(
                {
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                }); 

            $.datepicker.setDefaults($.datepicker.regional['en']); 

            var dateFormat = "mm/dd/yy",
                from = $("#<%= txtDate.ClientID %>")
                    .datepicker({
                        defaultDate: "+1w",
                        changeMonth: true,
                        dateFormat: 'mm/dd/yy',
                        autoClose: true                        
                        //numberOfMonths: 3
                    })
                    .on("change", function () {  
                        debugger
                        var startDt = $('#<%=hdStartDate.ClientID %>').val();
                        $('#<%=hdStartDate.ClientID %>').val(this.value);
                        var endDt = $('#<%=hdEndData.ClientID %>').val();
                        if (endDt === "") {
                            $('#<%=hdEndData.ClientID %>').val(this.value);
                            $('#<%=txtDateTo.ClientID %>').val(this.value);
                        }                         
                        var result = dateComparer()
                        if (result == true) {
                            to.datepicker("option", "minDate", getDate(this));
                        } else {
                            $('#<%=hdStartDate.ClientID %>').val(startDt);
                            $('#<%=txtDate.ClientID %>').val($('#<%=hdStartDate.ClientID %>').val());
                            messageFormSubmitted("The Start Date must be smaller than the End Date", "warning");
                        }                         
                    }),
                to = $("#<%= txtDateTo.ClientID %>").datepicker({
                    defaultDate: "+1w",
                    changeMonth: true,
                    dateFormat: 'mm/dd/yy',
                    autoClose: true                    
                    //numberOfMonths: 3
                })
                    .on("change", function () {       
                        debugger
                        var endDt = $('#<%=hdEndData.ClientID %>').val();
                        $('#<%=hdEndData.ClientID %>').val(this.value);
                        var result = dateComparer()
                        if (result == true) {
                            from.datepicker("option", "maxDate", getDate(this));
                        } else {
                            $('#<%=hdEndData.ClientID %>').val(endDt);
                            $('#<%=txtDateTo.ClientID %>').val($('#<%=hdEndData.ClientID %>').val());
                            messageFormSubmitted("The Start Date must be smaller than the End Date", "warning");
                        }                            
                    });

            function getDate(element) {
                debugger

                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }

           <%-- var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd3, collapse3);

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd4, collapse4);--%>

        })

        function pageLoad(event, args) {

            debugger

            $('#MainContent_txtDate').datepicker(
                {
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100',
                    onSelect: function (date) {
                        //debugger
                        var date2 = $("#<%= txtDate.ClientID %>").datepicker('getDate');
                        //console.log(date2);
                        //console.log(date2.setFullYear(date2.getFullYear() + 2));
                        date2.setFullYear(date2.getFullYear() + 2);
                        $("#<%= txtDateTo.ClientID %>").datepicker('setDate', date2);
                    }
                }); 

            $('#MainContent_txtDateTo').datepicker(
                {
                    //dateFormat: 'dd / mm / yy',
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                }); 

            $.datepicker.setDefaults($.datepicker.regional['en']);

            if (args.get_isPartialLoad()) {
                debugger   
            }

            <%--var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd3 = document.getElementById('<%=hiddenId3.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;--%>

            var hdName = document.getElementById('<%=hiddenName.ClientID%>').value;
            yesnoCheckCustom(hdName)

            //var collapse2 = document.getElementById('collapseOne_2');
            //afterDdlCheck(hd2, collapse2);

            //var collapse3 = document.getElementById('collapseOne_3');
            //afterDdlCheck(hd3, collapse3); 

            //var collapse4 = document.getElementById('collapseOne_4');
            //afterDdlCheck(hd4, collapse4);

            <%--var dt1 = document.getElementById('<%=txtDate.ClientID%>').value;
            $('#<%=txtDate.ClientID %>').datepicker("setDate", dt1);--%>

            var dateFormat = "mm/dd/yy",
                from = $("#<%= txtDate.ClientID %>")
                    .datepicker({
                        defaultDate: "+1w",
                        changeMonth: true,
                        dateFormat: 'mm/dd/yy',
                        autoClose: true                        
                        //numberOfMonths: 3
                    })
                    .on("change", function () {
                        debugger
                        var startDt = $('#<%=hdStartDate.ClientID %>').val();
                        $('#<%=hdStartDate.ClientID %>').val(this.value);
                        var endDt = $('#<%=hdEndData.ClientID %>').val();
                        if (endDt === "") {
                            $('#<%=hdEndData.ClientID %>').val(this.value);
                            $('#<%=txtDateTo.ClientID %>').val(this.value);
                        } 
                        var result = dateComparer()
                        if (result == true) {
                            to.datepicker("option", "minDate", getDate(this));
                        } else {
                            $('#<%=hdStartDate.ClientID %>').val(startDt);
                            $('#<%=txtDate.ClientID %>').val($('#<%=hdStartDate.ClientID %>').val());
                            messageFormSubmitted("The Start Date must be smaller than the End Date", "warning");
                        }                        
                    }),
                to = $("#<%= txtDateTo.ClientID %>").datepicker({
                    defaultDate: "+1w",
                    changeMonth: true,
                    dateFormat: 'mm/dd/yy',
                    autoClose: true
                    //numberOfMonths: 3
                })
                    .on("change", function () {   
                        debugger
                        var endDt = $('#<%=hdEndData.ClientID %>').val();
                        $('#<%=hdEndData.ClientID %>').val(this.value);
                        var result = dateComparer()
                        if (result == true) {
                            from.datepicker("option", "maxDate", getDate(this));
                        } else {
                            $('#<%=hdEndData.ClientID %>').val(endDt);
                            $('#<%=txtDateTo.ClientID %>').val($('#<%=hdEndData.ClientID %>').val());
                            messageFormSubmitted("The Start Date must be smaller than the End Date", "warning");
                        }                     
                    });

            function getDate(element) {
                debugger

                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }

        }

    </script>

</asp:Content>
