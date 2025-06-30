using Bidding.Models;
using System.Data.Common;
using System.Data;
using Bidding.Infrastructure.Extension;

namespace Bidding.Infrastructure
{
    public class EmailInfrasutructure : BaseInfrastructure ,IEmailInfrasutructure
    {
        public EmailInfrasutructure(IConfiguration configuration) : base(configuration)
        {
        }
        #region Constants
        private const string EmailTemplateCodeStoredProcedureName = "EmailTemplateCode";
        private const string EmailcodeByEmailstatusidCodeStoredProcedureName = "EmailCodeEmailStatusId";


        //Column names
        private const string IdColumnName = "Id";
        private const string CodeColumnName = "Code";
        private const string SubjectColumnName = "Subject";
        private const string BodyColumnName = "Body";
        private const string ToColumn = "To";
        private const string FromColumnName = "EndDate";
        private const string ActiveColumnName = "Active";
        private const string EmailstatusidColoumName = "Emailstatusid";
        private const string EmailCodeColoumName = "EmailCode";
        //Parameter names
        private const string IdParameterName = "PId";
        private const string ActiveParameterName = "PActive";
        private const string CodeParameterName = "PCode";
        private const string EmailstatusidParameterName = "PEmailstatusid";
        public string code;

        //private const string SubjectParameterName = "PSubject";
        //private const string TagsParameterName = "PTags";
        //private const string StartDateParameterName = "PStartDate";
        //private const string EndDateParameterName = "PEndDate";

        //private const string PageNoParameterName = "PPageNo";
        //private const string LimitParameterName = "PLimit";
        //private const string TotalCountParameterName = "PTotalCount";

        #endregion

        #region methods

        public async Task<EmailT> GetTemplateBycode(string code)
        {
            EmailT emailT = new EmailT();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailInfrasutructure.CodeParameterName, code)
            };

            using (var dataReader = await base.ExecuteReader(parameters, EmailInfrasutructure.EmailTemplateCodeStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        emailT = new EmailT
                        {
                            Subject = dataReader.GetStringValue(EmailInfrasutructure.SubjectColumnName),
                            Body = dataReader.GetStringValue(EmailInfrasutructure.BodyColumnName),
                            Active = dataReader.GetBooleanValue(EmailInfrasutructure.ActiveColumnName)
                        };
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return emailT;
        }
        public async Task<string> GetCodeByStatusid(int EmailStatusId)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailInfrasutructure.EmailstatusidParameterName, EmailStatusId )
            };

            using (var dataReader = await base.ExecuteReader(parameters, EmailInfrasutructure.EmailcodeByEmailstatusidCodeStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                    
                       code = dataReader.GetStringValue(EmailInfrasutructure.EmailCodeColoumName);
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }

            }
            return code;
        }











        #endregion
    }

}
