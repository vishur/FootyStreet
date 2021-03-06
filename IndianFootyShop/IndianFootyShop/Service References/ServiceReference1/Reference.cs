﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IndianFootyShop.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd", ConfigurationName="ServiceReference1.IAppointmentService")]
    public interface IAppointmentService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd/IAppointmentService/SubmitApp" +
            "ointment", ReplyAction="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd/IAppointmentService/SubmitApp" +
            "ointmentResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        IndianFootyShop.ServiceReference1.SubmitAppointmentResponse SubmitAppointment(IndianFootyShop.ServiceReference1.SubmitAppointmentRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd/IAppointmentService/SubmitApp" +
            "ointment", ReplyAction="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd/IAppointmentService/SubmitApp" +
            "ointmentResponse")]
        System.Threading.Tasks.Task<IndianFootyShop.ServiceReference1.SubmitAppointmentResponse> SubmitAppointmentAsync(IndianFootyShop.ServiceReference1.SubmitAppointmentRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class SubmitAppointmentAppointment : object, System.ComponentModel.INotifyPropertyChanged {
        
        private PersonToBeInterviewed[] personToBeInterviewedField;
        
        private RequestedBy requestedByField;
        
        private Address addressField;
        
        private System.Nullable<int> appointmentIdField;
        
        private bool appointmentIdFieldSpecified;
        
        private System.Nullable<int> caseNumberField;
        
        private bool caseNumberFieldSpecified;
        
        private System.Nullable<int> individualIdField;
        
        private bool individualIdFieldSpecified;
        
        private System.Nullable<int> addressIdField;
        
        private bool addressIdFieldSpecified;
        
        private int officeIdField;
        
        private string catagoryCodeField;
        
        private System.DateTime startDateField;
        
        private System.Nullable<System.DateTime> endDateField;
        
        private bool endDateFieldSpecified;
        
        private string firstNameField;
        
        private string lastNameField;
        
        private string middleInitialField;
        
        private string suffixCodeField;
        
        private string interviewTypeCodeField;
        
        private string specialAccomodationCodeField;
        
        private string appointmentStatusCodeField;
        
        private string appointmentNotesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PersonToBeInterviewed", Order=0)]
        public PersonToBeInterviewed[] PersonToBeInterviewed {
            get {
                return this.personToBeInterviewedField;
            }
            set {
                this.personToBeInterviewedField = value;
                this.RaisePropertyChanged("PersonToBeInterviewed");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public RequestedBy RequestedBy {
            get {
                return this.requestedByField;
            }
            set {
                this.requestedByField = value;
                this.RaisePropertyChanged("RequestedBy");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public Address Address {
            get {
                return this.addressField;
            }
            set {
                this.addressField = value;
                this.RaisePropertyChanged("Address");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=3)]
        public System.Nullable<int> AppointmentId {
            get {
                return this.appointmentIdField;
            }
            set {
                this.appointmentIdField = value;
                this.RaisePropertyChanged("AppointmentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AppointmentIdSpecified {
            get {
                return this.appointmentIdFieldSpecified;
            }
            set {
                this.appointmentIdFieldSpecified = value;
                this.RaisePropertyChanged("AppointmentIdSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public System.Nullable<int> CaseNumber {
            get {
                return this.caseNumberField;
            }
            set {
                this.caseNumberField = value;
                this.RaisePropertyChanged("CaseNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CaseNumberSpecified {
            get {
                return this.caseNumberFieldSpecified;
            }
            set {
                this.caseNumberFieldSpecified = value;
                this.RaisePropertyChanged("CaseNumberSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public System.Nullable<int> IndividualId {
            get {
                return this.individualIdField;
            }
            set {
                this.individualIdField = value;
                this.RaisePropertyChanged("IndividualId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IndividualIdSpecified {
            get {
                return this.individualIdFieldSpecified;
            }
            set {
                this.individualIdFieldSpecified = value;
                this.RaisePropertyChanged("IndividualIdSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=6)]
        public System.Nullable<int> AddressId {
            get {
                return this.addressIdField;
            }
            set {
                this.addressIdField = value;
                this.RaisePropertyChanged("AddressId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AddressIdSpecified {
            get {
                return this.addressIdFieldSpecified;
            }
            set {
                this.addressIdFieldSpecified = value;
                this.RaisePropertyChanged("AddressIdSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public int OfficeId {
            get {
                return this.officeIdField;
            }
            set {
                this.officeIdField = value;
                this.RaisePropertyChanged("OfficeId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string CatagoryCode {
            get {
                return this.catagoryCodeField;
            }
            set {
                this.catagoryCodeField = value;
                this.RaisePropertyChanged("CatagoryCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public System.DateTime StartDate {
            get {
                return this.startDateField;
            }
            set {
                this.startDateField = value;
                this.RaisePropertyChanged("StartDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=10)]
        public System.Nullable<System.DateTime> EndDate {
            get {
                return this.endDateField;
            }
            set {
                this.endDateField = value;
                this.RaisePropertyChanged("EndDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EndDateSpecified {
            get {
                return this.endDateFieldSpecified;
            }
            set {
                this.endDateFieldSpecified = value;
                this.RaisePropertyChanged("EndDateSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=11)]
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
                this.RaisePropertyChanged("FirstName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=12)]
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
                this.RaisePropertyChanged("LastName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=13)]
        public string MiddleInitial {
            get {
                return this.middleInitialField;
            }
            set {
                this.middleInitialField = value;
                this.RaisePropertyChanged("MiddleInitial");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=14)]
        public string SuffixCode {
            get {
                return this.suffixCodeField;
            }
            set {
                this.suffixCodeField = value;
                this.RaisePropertyChanged("SuffixCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=15)]
        public string InterviewTypeCode {
            get {
                return this.interviewTypeCodeField;
            }
            set {
                this.interviewTypeCodeField = value;
                this.RaisePropertyChanged("InterviewTypeCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=16)]
        public string SpecialAccomodationCode {
            get {
                return this.specialAccomodationCodeField;
            }
            set {
                this.specialAccomodationCodeField = value;
                this.RaisePropertyChanged("SpecialAccomodationCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=17)]
        public string AppointmentStatusCode {
            get {
                return this.appointmentStatusCodeField;
            }
            set {
                this.appointmentStatusCodeField = value;
                this.RaisePropertyChanged("AppointmentStatusCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=18)]
        public string AppointmentNotes {
            get {
                return this.appointmentNotesField;
            }
            set {
                this.appointmentNotesField = value;
                this.RaisePropertyChanged("AppointmentNotes");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class PersonToBeInterviewed : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int appointmentIdField;
        
        private int individualIdField;
        
        private string firstNameField;
        
        private string lastNameField;
        
        private string middleInitialField;
        
        private string suffixCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int AppointmentId {
            get {
                return this.appointmentIdField;
            }
            set {
                this.appointmentIdField = value;
                this.RaisePropertyChanged("AppointmentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int IndividualId {
            get {
                return this.individualIdField;
            }
            set {
                this.individualIdField = value;
                this.RaisePropertyChanged("IndividualId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
                this.RaisePropertyChanged("FirstName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=3)]
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
                this.RaisePropertyChanged("LastName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string MiddleInitial {
            get {
                return this.middleInitialField;
            }
            set {
                this.middleInitialField = value;
                this.RaisePropertyChanged("MiddleInitial");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string SuffixCode {
            get {
                return this.suffixCodeField;
            }
            set {
                this.suffixCodeField = value;
                this.RaisePropertyChanged("SuffixCode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class SSPDCMessageType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private SSPDCMessageTypeMessageClassification messageClassificationField;
        
        private string activityDurationField;
        
        private string messageKeyField;
        
        private string messageTextField;
        
        private string processIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public SSPDCMessageTypeMessageClassification messageClassification {
            get {
                return this.messageClassificationField;
            }
            set {
                this.messageClassificationField = value;
                this.RaisePropertyChanged("messageClassification");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string activityDuration {
            get {
                return this.activityDurationField;
            }
            set {
                this.activityDurationField = value;
                this.RaisePropertyChanged("activityDuration");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string messageKey {
            get {
                return this.messageKeyField;
            }
            set {
                this.messageKeyField = value;
                this.RaisePropertyChanged("messageKey");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string messageText {
            get {
                return this.messageTextField;
            }
            set {
                this.messageTextField = value;
                this.RaisePropertyChanged("messageText");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string processId {
            get {
                return this.processIdField;
            }
            set {
                this.processIdField = value;
                this.RaisePropertyChanged("processId");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public enum SSPDCMessageTypeMessageClassification {
        
        /// <remarks/>
        confirm,
        
        /// <remarks/>
        errors,
        
        /// <remarks/>
        warning,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class ResponseType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string runModeField;
        
        private System.Nullable<int> appointmentIdField;
        
        private bool appointmentIdFieldSpecified;
        
        private string caseNumberField;
        
        private string documentIdField;
        
        private string exceptionIdField;
        
        private string transactionDurationField;
        
        private SSPDCMessageType[] messagesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=0)]
        public string runMode {
            get {
                return this.runModeField;
            }
            set {
                this.runModeField = value;
                this.RaisePropertyChanged("runMode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=1)]
        public System.Nullable<int> AppointmentId {
            get {
                return this.appointmentIdField;
            }
            set {
                this.appointmentIdField = value;
                this.RaisePropertyChanged("AppointmentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AppointmentIdSpecified {
            get {
                return this.appointmentIdFieldSpecified;
            }
            set {
                this.appointmentIdFieldSpecified = value;
                this.RaisePropertyChanged("AppointmentIdSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string caseNumber {
            get {
                return this.caseNumberField;
            }
            set {
                this.caseNumberField = value;
                this.RaisePropertyChanged("caseNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string documentId {
            get {
                return this.documentIdField;
            }
            set {
                this.documentIdField = value;
                this.RaisePropertyChanged("documentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string exceptionId {
            get {
                return this.exceptionIdField;
            }
            set {
                this.exceptionIdField = value;
                this.RaisePropertyChanged("exceptionId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string transactionDuration {
            get {
                return this.transactionDurationField;
            }
            set {
                this.transactionDurationField = value;
                this.RaisePropertyChanged("transactionDuration");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("messages", Order=6)]
        public SSPDCMessageType[] messages {
            get {
                return this.messagesField;
            }
            set {
                this.messagesField = value;
                this.RaisePropertyChanged("messages");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class Address : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string addressLine1Field;
        
        private string addressLine2Field;
        
        private string cityField;
        
        private string stateCodeField;
        
        private string zipCode4Field;
        
        private string countyCodeField;
        
        private string zipCode5Field;
        
        private System.Nullable<YesNoType> isValidatedField;
        
        private bool isValidatedFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string AddressLine1 {
            get {
                return this.addressLine1Field;
            }
            set {
                this.addressLine1Field = value;
                this.RaisePropertyChanged("AddressLine1");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=1)]
        public string AddressLine2 {
            get {
                return this.addressLine2Field;
            }
            set {
                this.addressLine2Field = value;
                this.RaisePropertyChanged("AddressLine2");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
                this.RaisePropertyChanged("City");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string StateCode {
            get {
                return this.stateCodeField;
            }
            set {
                this.stateCodeField = value;
                this.RaisePropertyChanged("StateCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string ZipCode4 {
            get {
                return this.zipCode4Field;
            }
            set {
                this.zipCode4Field = value;
                this.RaisePropertyChanged("ZipCode4");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string CountyCode {
            get {
                return this.countyCodeField;
            }
            set {
                this.countyCodeField = value;
                this.RaisePropertyChanged("CountyCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=6)]
        public string ZipCode5 {
            get {
                return this.zipCode5Field;
            }
            set {
                this.zipCode5Field = value;
                this.RaisePropertyChanged("ZipCode5");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=7)]
        public System.Nullable<YesNoType> IsValidated {
            get {
                return this.isValidatedField;
            }
            set {
                this.isValidatedField = value;
                this.RaisePropertyChanged("IsValidated");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsValidatedSpecified {
            get {
                return this.isValidatedFieldSpecified;
            }
            set {
                this.isValidatedFieldSpecified = value;
                this.RaisePropertyChanged("IsValidatedSpecified");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public enum YesNoType {
        
        /// <remarks/>
        Y,
        
        /// <remarks/>
        N,
        
        /// <remarks/>
        U,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd")]
    public partial class RequestedBy : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int appointmentIdField;
        
        private int individualIdField;
        
        private string firstNameField;
        
        private string lastNameField;
        
        private string middleInitialField;
        
        private string suffixCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int AppointmentId {
            get {
                return this.appointmentIdField;
            }
            set {
                this.appointmentIdField = value;
                this.RaisePropertyChanged("AppointmentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int IndividualId {
            get {
                return this.individualIdField;
            }
            set {
                this.individualIdField = value;
                this.RaisePropertyChanged("IndividualId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
                this.RaisePropertyChanged("FirstName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=3)]
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
                this.RaisePropertyChanged("LastName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string MiddleInitial {
            get {
                return this.middleInitialField;
            }
            set {
                this.middleInitialField = value;
                this.RaisePropertyChanged("MiddleInitial");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string SuffixCode {
            get {
                return this.suffixCodeField;
            }
            set {
                this.suffixCodeField = value;
                this.RaisePropertyChanged("SuffixCode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SubmitAppointment", WrapperNamespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd", IsWrapped=true)]
    public partial class SubmitAppointmentRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd", Order=0)]
        public IndianFootyShop.ServiceReference1.SubmitAppointmentAppointment Appointment;
        
        public SubmitAppointmentRequest() {
        }
        
        public SubmitAppointmentRequest(IndianFootyShop.ServiceReference1.SubmitAppointmentAppointment Appointment) {
            this.Appointment = Appointment;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SubmitAppointmentResponse", WrapperNamespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd", IsWrapped=true)]
    public partial class SubmitAppointmentResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://hbe.ky.gov/xsd/INT_PUSH_AR_APPL_REG_V1.0.xsd", Order=0)]
        public IndianFootyShop.ServiceReference1.ResponseType SubmitAppointmentResult;
        
        public SubmitAppointmentResponse() {
        }
        
        public SubmitAppointmentResponse(IndianFootyShop.ServiceReference1.ResponseType SubmitAppointmentResult) {
            this.SubmitAppointmentResult = SubmitAppointmentResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAppointmentServiceChannel : IndianFootyShop.ServiceReference1.IAppointmentService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AppointmentServiceClient : System.ServiceModel.ClientBase<IndianFootyShop.ServiceReference1.IAppointmentService>, IndianFootyShop.ServiceReference1.IAppointmentService {
        
        public AppointmentServiceClient() {
        }
        
        public AppointmentServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AppointmentServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AppointmentServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AppointmentServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public IndianFootyShop.ServiceReference1.SubmitAppointmentResponse SubmitAppointment(IndianFootyShop.ServiceReference1.SubmitAppointmentRequest request) {
            return base.Channel.SubmitAppointment(request);
        }
        
        public System.Threading.Tasks.Task<IndianFootyShop.ServiceReference1.SubmitAppointmentResponse> SubmitAppointmentAsync(IndianFootyShop.ServiceReference1.SubmitAppointmentRequest request) {
            return base.Channel.SubmitAppointmentAsync(request);
        }
    }
}
