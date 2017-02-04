import sqlitedb as db
import util

def is_email_in_db(conversationID, entryID):
    id = get_email_id_for_these(conversationID, entryID)
    if (id == '?'):
        return False
    return True
	
def get_email_id_for_these(conversationID, entryID):
    query = "SELECT Emails.ID from Emails WHERE Emails.ConversationID='{0}' and Emails.EntryID='{1}';".format(conversationID, entryID)
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)

def get_email_id_for_email(entryID):
    query = "SELECT Emails.ID from Emails WHERE Emails.EntryID='{0}';".format(entryID)
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)
	
def add_email(conversationID, entryID):
    query = "INSERT INTO Emails (EntryID, ConversationID) VALUES ('{0}','{1}');".format(entryID, conversationID)
    return db.insert_and_get_id(query, 'Emails')
	