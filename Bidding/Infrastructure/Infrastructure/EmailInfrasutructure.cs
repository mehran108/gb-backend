using GoldBank.Models;
using System.Data.Common;
using System.Data;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class EmailInfrasutructure : BaseInfrastructure, IEmailInfrasutructure
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
                GetParameter(CodeParameterName, code)
            };

            using (var dataReader = await ExecuteReader(parameters, EmailTemplateCodeStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        emailT = new EmailT
                        {
                            Subject = dataReader.GetStringValue(SubjectColumnName),
                            Body = dataReader.GetStringValue(BodyColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
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
                GetParameter(EmailstatusidParameterName, EmailStatusId )
            };

            using (var dataReader = await ExecuteReader(parameters, EmailcodeByEmailstatusidCodeStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {

                        code = dataReader.GetStringValue(EmailCodeColoumName);
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
