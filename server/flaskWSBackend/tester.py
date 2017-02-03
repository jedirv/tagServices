import flaskWSBackend as api
import json

import os
import flask
import unittest
import tempfile

class FlaskrTestCase(unittest.TestCase):

    def setUp(self):
        self.db_fd, flask.app.config['DATABASE'] = tempfile.mkstemp()
        flask.app.config['TESTING'] = True
        self.app = flaskr.app.test_client()
        with flask.app.app_context():
            flask.init_db()

    def tearDown(self):
        os.close(self.db_fd)
        os.unlink(flask.app.config['DATABASE'])

    def test_empty_db(self):
        rv = self.app.get('/')
        assert b'No entries here so far' in rv.data
        
if __name__ == '__main__':
    unittest.main()

#def run_tests():
#    json_result = api.add_tag('tag1')
#    result = json.loads(json_result)
#    print(result)
    
   