import sqlitedb as db


def is_email_in_db(conversationID, entryID):
    all_rows = get_email_ID_for_these(conversationID, entryID)
	if (all_rows):
	    return True
    return False
	
def get_email_ID_for_these(conversationID, entryID):
    query = "SELECT Emails.ID from Emails WHERE Emails.ConversationID='{0}' and Emails.EntryID='{1}';".format(conversationID, entryID)
    all_rows = db.query_db(query)
    return all_rows
	
def add_email(conversationID, entryID):
    query = "INSERT INTO Emails (EntryID, ConversationID) VALUES ('{0}','{1}');".format(entryID, conversationID)
    return db.insert_and_get_id(query, 'Emails')
	