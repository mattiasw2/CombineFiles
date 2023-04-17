using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Xunit;

namespace TestCombineFIles
{
    public class TestRemoveEmptyLines {
        private string test = @"    void Update(T entity, bool noSaveChanges = false){};
    
    
    
    
    
    void UpdateAndAttach(T entity, bool noSaveChanges = false){};
    
    void Remove(T entity, bool noSaveChanges = false){};
    
    void Remove(string id, bool noSaveChanges = false){};
    
    void Remove(Expression<Func<T, bool>> predicate, bool noSaveChanges = false){};
// Combined from: C:\data3\ssc\sss7\MyDb\IDbAccountRepo.cs

// Combined from: C:\data3\ssc\sss7\MyDb\IDbBlobRepo.cs
    DdBlob? GetByHashValue(string hash, string accountid){};
// Combined from: C:\data3\ssc\sss7\MyDb\IDbCommentRepo.cs
    DdComment? GetByInstanceId(string hash, string accountid){};
// Combined from: C:\data3\ssc\sss7\MyDb\IDbFormRepo.cs
    DdForm? GetBySpreadsheet(string accountid, string spreadsheet){};
// Combined from: C:\data3\ssc\sss7\MyDb\IDbInstanceRepo.cs

// Combined from: C:\data3\ssc\sss7\MyDb\IDbUserRepo.cs
    DdUser? FindbyEmail(string email){};
// Combined from: C:\data3\ssc\sss7\MyDb\IUow.cs
IDbUserRepo Users
IDbInstanceRepo Instances
IDbFormRepo Forms
IDbBlobRepo Blobs
IDbCommentRepo Comments
IDbAccountRepo Accounts
DbContext Db
        bool HasUnsavedChanges(){};
// Combined from: C:\data3\ssc\sss7\MyDbContexts\Fields\BuildAndFillForm.cs
IGetField _getField
string _aspPage
Regex regexGetFieldText
Regex regexGetFieldCheckbox
Regex regexGetFieldTextArea
Regex regexGetFieldRadio
Regex regexGetFieldSelect
        
        
        
        
        public string GetWebPage()
{
// Convert.ToInt32(..);
// _aspPage.IndexOf(..);
// res.Append(..);
// _aspPage.Substring(..);
// _aspPage.IndexOf(..);
// _aspPage.Substring(..);
// InterpretTag(..);
// HandleSpecialCharacters(..);
// res.Append(..);
// res.Append(..);
// _aspPage.Substring(..);
// res.ToString(..);}
        public string HandleSpecialCharacters(string data)
{
// data.Replace(..);}
        
        
        
        
        
        
        private string InterpretTag(string tag)
{
// regexGetFieldText.Match(..);
// _getField.GetFieldText(..);
// match.Groups[1].ToString(..);
// regexGetFieldSelect.Match(..);
// _getField.GetFieldSelect(..);";

        [Fact]
        public void TestRemoveEmptyLines1() {
            var res = Core.RemoveEmptyLines(test);
            Assert.False(res.Split('\n').Length == 14);
        }
    }
}
